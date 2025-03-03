using System.Text.Json;
using Fleck;

namespace BagarIo;

class GameManager
{
    private readonly Dictionary<IWebSocketConnection, GameObject> ClientsList = new();
    private List<GameObject> GameObjects = new();
    private readonly int GameObjectNumber = 2;
    private readonly int MapHeight = 4000;
    private readonly int MapWidth = 4000;

    private readonly Random rand = new();

    public GameManager()
    {
        InitializeGameObjects();
    }

    private void InitializeGameObjects()
    {
        for (int i = 0; i < GameObjectNumber; i++)
        {
            GameObjects.Add(
                new GameObject
                {
                    R = rand.Next(256),
                    G = rand.Next(256),
                    B = rand.Next(256),
                    A = rand.Next(256),
                    PosX = rand.Next(MapWidth),
                    PosY = rand.Next(MapHeight),
                    Radius = 10,
                    Type = GameObjectType.Default,
                }
            );
        }
    }

    private bool IsPointInCircle(GameObject point, GameObject circle)
    {
        float dx = point.PosX - circle.PosX;
        float dy = point.PosY - circle.PosY;
        return dx * dx + dy * dy <= circle.Radius * circle.Radius;
    }

    public void UpdateGameData()
    {
        foreach (var obj in GameObjects.ToList())
        {
            foreach (var player in ClientsList.Values)
            {
                if (IsPointInCircle(obj, player))
                {
                    GameObjects.Remove(obj);
                    player.Radius += obj.Radius / 10;
                }
            }
        }
    }

    public void AddNewClient(IWebSocketConnection socket)
    {
        if (!ClientsList.ContainsKey(socket))
        {
            GameObject newPlayer = new()
            {
                PosX = rand.Next(MapWidth),
                PosY = rand.Next(MapHeight),
                Radius = 20,
                Type = GameObjectType.Player,
                Id = ClientsList.Count + 1,
            };

            ClientsList[socket] = newPlayer;

            socket.Send(
                JsonSerializer.Serialize(
                    new ConnectResponse(newPlayer.Id ?? 0, MapWidth, MapHeight)
                )
            );
        }
    }

    public void RemoveClient(IWebSocketConnection socket)
    {
        ClientsList.Remove(socket);
    }

    public void SendClientsGameData()
    {
        var gameData = ClientsList.Values.Concat(GameObjects).ToList();
        var result = JsonSerializer.Serialize(gameData);

        foreach (var client in ClientsList.Keys)
        {
            client.Send(result);
        }
    }

    public void UpdateClientList(IWebSocketConnection socket, GameObject? player)
    {
        if (player == null)
            return;

        if (ClientsList.TryGetValue(socket, out var existingClient))
        {
            existingClient.PosX = player.PosX;
            existingClient.PosY = player.PosY;
            existingClient.Radius = player.Radius;
        }
    }
}
