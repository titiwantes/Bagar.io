using System.Text.Json;
using Fleck;

namespace BagarIo;

class WebSocketHandler
{
    private readonly GameManager gameManager = new();
    private readonly WebSocketServer server;

    public WebSocketHandler(string address)
    {
        server = new WebSocketServer(address);
    }

    public void Start()
    {
        server.Start(socket =>
        {
            socket.OnOpen = () =>
            {
                Console.WriteLine("New client connected!");
                gameManager.AddNewClient(socket);
            };

            socket.OnClose = () =>
            {
                socket.Close();
                Console.WriteLine("Client disconnected.");
                gameManager.RemoveClient(socket);
            };

            socket.OnMessage = message =>
            {
                try
                {
                    var player = JsonSerializer.Deserialize<GameObject>(message);
                    gameManager.UpdateClientList(socket, player);
                    gameManager.UpdateGameData();
                }
                catch
                {
                    Console.WriteLine("Failed to deserialize JSON");
                }
                gameManager.SendClientsGameData();
            };
        });

        Console.WriteLine($"WebSocket server started on {server.Location}");
    }
}
