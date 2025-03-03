namespace BagarIo.Models;

using System.Numerics;
using Raylib_cs;

class Player
{
    public int Id { get; set; }
    public float Radius { get; set; }
    private readonly Random rand = new();
    private Raylib_cs.Color color;
    private Vector2 Position;

    public Player()
    {
        color = new(
            (byte)(rand.Next() % 255),
            (byte)(rand.Next() % 255),
            (byte)(rand.Next() % 255)
        );
        Id = rand.Next();
    }

    public void Display() => Raylib.DrawCircleV(Position, Radius, color);

    public void Move(Vector2 position, Vector2 mousePosition, int screenWidth, int screenHeight)
    {
        Position.X = (mousePosition.X - (screenWidth / 2)) * 10 / (screenWidth / 2) + position.X;
        Position.Y = (mousePosition.Y - (screenHeight / 2)) * 10 / (screenHeight / 2) + position.Y;
    }

    public GameObject GetPlayerData() =>
        new GameObject
        {
            Id = Id,
            PosX = (int)Position.X,
            PosY = (int)Position.Y,
            Radius = Radius,
            R = color.R,
            G = color.G,
            B = color.B,
            A = color.A,
            Type = GameObjectType.Player,
        };
}
