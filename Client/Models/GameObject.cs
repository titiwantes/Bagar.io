namespace BagarIo.Models;

enum GameObjectType
{
    Default,
    Player,
}

class GameObject
{
    public int PosX { get; set; }
    public int PosY { get; set; }
    public int R { get; set; }
    public int G { get; set; }
    public int B { get; set; }
    public int A { get; set; }
    public float Radius { get; set; }
    public GameObjectType Type { get; set; }
    public int? Id { get; set; }
}
