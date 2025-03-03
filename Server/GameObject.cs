namespace BagarIo;

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

    public GameObject() {}

    public GameObject(int posX, int posY, float radius, GameObjectType type = GameObjectType.Default)
    {
        PosX = posX;
        PosY = posY;
        Radius = radius;
        Type = type;
    }

    public bool IsCollidingWith(GameObject other)
    {
        float dx = PosX - other.PosX;
        float dy = PosY - other.PosY;
        float distanceSquared = dx * dx + dy * dy;
        float radiusSum = Radius + other.Radius;
        return distanceSquared <= radiusSum * radiusSum;
    }
}
