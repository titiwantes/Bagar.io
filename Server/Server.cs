using System.Text.Json;
using Fleck;
namespace BagarIo;

class Server
{
    private readonly WebSocketServer _webSocketServer;
    private readonly GameManager _gameManager;

    public Server(string url)
    {
        _webSocketServer = new WebSocketServer(url);
        _gameManager = new GameManager();
    }

    public void Start()
    {
        _webSocketServer.Start(socket =>
        {
            socket.OnOpen = () =>
            {
                Console.WriteLine($"New client connected: {socket.ConnectionInfo.ClientIpAddress}");
                _gameManager.AddNewClient(socket);
            };

            socket.OnClose = () =>
            {
                Console.WriteLine("Client disconnected.");
                _gameManager.RemoveClient(socket);
            };

            socket.OnMessage = message =>
            {
                try
                {
                    var player = JsonSerializer.Deserialize<GameObject>(message);
                    _gameManager.UpdateClientList(socket, player);
                    _gameManager.UpdateGameData();
                }
                catch
                {
                    Console.WriteLine("Failed to deserialize JSON.");
                }

                _gameManager.SendClientsGameData();
            };
        });

        Console.WriteLine($"WebSocket server started at {_webSocketServer.Location}");
    }
}
