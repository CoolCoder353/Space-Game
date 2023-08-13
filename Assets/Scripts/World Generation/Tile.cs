
public class Tile
{
    private TileType type;

    public TileType GetTileType()
    {
        return type;
    }

    public Tile(TileType type)
    {
        this.type = type;
    }
}


public enum TileType
{
    water,
    grass
}
