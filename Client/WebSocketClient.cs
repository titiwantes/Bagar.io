namespace BagarIo;

using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using BagarIo.Models;

class WebSocketClient
{
    private readonly ClientWebSocket _ws = new();
    private readonly Uri _serverUri;

    public WebSocketClient(Uri serverUri) => _serverUri = serverUri;

    public async Task<ConnectResponse?> ConnectServer()
    {
        try
        {
            Console.WriteLine("Connecting to server...");
            await _ws.ConnectAsync(_serverUri, CancellationToken.None);
            Console.WriteLine("Connected!");
            var message = await ReceiveMessage();
            return message != null ? JsonSerializer.Deserialize<ConnectResponse>(message) : null;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
            return null;
        }
    }

    private async Task<string?> ReceiveMessage()
    {
        byte[] buffer = new byte[1024];
        var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        return result.MessageType == WebSocketMessageType.Text
            ? Encoding.UTF8.GetString(buffer, 0, result.Count)
            : null;
    }

    public async Task DisconnectServer()
    {
        if (_ws.State == WebSocketState.Open)
        {
            await _ws.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Closing",
                CancellationToken.None
            );
            Console.WriteLine("WebSocket closed.");
        }
    }

    public async Task SendPlayerData(GameObject data)
    {
        string jsonData = JsonSerializer.Serialize(data);
        await _ws.SendAsync(
            new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonData)),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None
        );
    }

    public async Task GetMapData(List<GameObject> mapData, Player player)
    {
        var buffer = new ArraySegment<byte>(new byte[4096]);
        var memoryStream = new MemoryStream();

        while (_ws.State == WebSocketState.Open)
        {
            try
            {
                WebSocketReceiveResult result;
                do
                {
                    result = await _ws.ReceiveAsync(buffer, CancellationToken.None);
                    if (buffer.Array != null)
                    {
                        memoryStream.Write(buffer.Array, buffer.Offset, result.Count);
                    }
                } while (!result.EndOfMessage);

                string jsonMessage = Encoding.UTF8.GetString(memoryStream.ToArray());
                memoryStream.SetLength(0);

                var data = JsonSerializer.Deserialize<List<GameObject>>(jsonMessage);
                if (data != null)
                {
                    lock (mapData)
                    {
                        mapData.Clear();
                        mapData.AddRange(data);
                    }
                    foreach (var obj in data)
                    {
                        if (obj.Id == player.Id)
                        {
                            player.Radius = obj.Radius;
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while getting map data {e}");
            }
        }
    }
}
