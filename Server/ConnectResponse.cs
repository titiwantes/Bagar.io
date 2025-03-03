namespace BagarIo;

class ConnectResponse
{
    public int Id { get; set; }
    public int MapWidth { get; set; }
    public int MapHeight { get; set; }

    public ConnectResponse(int id, int mapWidth, int mapHeight)
    {
        Id = id;
        MapWidth = mapWidth;
        MapHeight = mapHeight;
    }
}
