using System.Numerics;
using System.Text.Json;
using BagarIo.Models;
using Raylib_cs;

class GameManager
{
    static readonly int ScreenWidth = 1280;
    static readonly int ScreenHeight = 720;
    static int MapWidth;
    static int MapHeight;
    static readonly int GridSize = 50;
    static Vector2 mousePosition;
    static Player User = new();
    static Camera2D camera = new()
    {
        Target = new Vector2(0, 0),
        Offset = new Vector2(ScreenWidth / 2, ScreenHeight / 2),
        Rotation = 0.0f,
        Zoom = 1.0f,
    };
    static readonly BagarIo.WebSocketClient Client = new(new Uri("ws://localhost:8181"));
    static List<GameObject> MapData = [];

    static void HandleMousePosition()
    {
        mousePosition = Raylib.GetMousePosition();
        camera.Target.X = Math.Clamp(
            camera.Target.X + (mousePosition.X - (ScreenWidth / 2)) * 10 / (ScreenWidth / 2),
            0,
            MapWidth
        );
        camera.Target.Y = Math.Clamp(
            camera.Target.Y + (mousePosition.Y - (ScreenHeight / 2)) * 10 / (ScreenHeight / 2),
            0,
            MapHeight
        );
    }

    static void DisplayMap()
    {
        Raylib.ClearBackground(Raylib_cs.Color.White);
        int minX = -ScreenWidth / 2,
            maxX = MapWidth + ScreenWidth / 2;
        int minY = -ScreenHeight / 2,
            maxY = MapHeight + ScreenHeight / 2;

        for (int x = minX; x <= maxX; x += GridSize)
            Raylib.DrawLine(x, minY, x, maxY, Raylib_cs.Color.LightGray);
        for (int y = minY; y <= maxY; y += GridSize)
            Raylib.DrawLine(minX, y, maxX, y, Raylib_cs.Color.LightGray);
    }

    static void DisplayMapData()
    {
        lock (MapData)
        {
            foreach (var obj in MapData)
            {
                Raylib_cs.Color c = new((byte)obj.R, (byte)obj.G, (byte)obj.B, (byte)obj.A);
                Raylib.DrawCircle(obj.PosX, obj.PosY, obj.Radius, c);
            }
        }
    }

    public static async Task Run()
    {
        ConnectResponse? response = await Client.ConnectServer();
        if (response == null)
            Environment.Exit(1);
        User.Id = response.Id;
        User.Radius = 50;
        MapHeight = response.MapHeight;
        MapWidth = response.MapWidth;

        Raylib.InitWindow(ScreenWidth, ScreenHeight, "Bagar.io");
        Raylib.SetTargetFPS(60);

        _ = Task.Run(() => Client.GetMapData(MapData, User));

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.BeginMode2D(camera);
            DisplayMap();
            DisplayMapData();
            User.Move(camera.Target, mousePosition, ScreenWidth, ScreenHeight);
            User.Display();
            HandleMousePosition();
            await Client.SendPlayerData(User.GetPlayerData());
            Raylib.EndMode2D();
            Raylib.EndDrawing();
        }

        await Client.DisconnectServer();
        Raylib.CloseWindow();
    }
}
