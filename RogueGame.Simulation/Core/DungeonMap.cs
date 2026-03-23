namespace RogueGame.Simulation.Core;

public struct Tile
{
    public char Glyph;
    public (byte R, byte G, byte B) Foreground;
    public bool IsWalkable;
    public bool IsExplored;

    public static Tile Floor => new Tile
    {
        Glyph = '.',
        Foreground = (100, 100, 100),
        IsWalkable = true,
        IsExplored = false
    };

    public static Tile Wall => new Tile
    {
        Glyph = '#',
        Foreground = (180, 180, 180),
        IsWalkable = false,
        IsExplored = false
    };
}

public class DungeonMap
{
    public int Width { get; }
    public int Height { get; }

    private readonly Tile[] _tiles;

    public DungeonMap(int width, int height)
    {
        Width = width;
        Height = height;
        _tiles = new Tile[width * height];

        Initialise();
    }

    private void Initialise()
    {
        // Fill with floor tiles
        for (int i = 0; i < _tiles.Length; i++)
            _tiles[i] = Tile.Floor;

        // Draw border walls
        for (int x = 0; x < Width; x++)
        {
            SetTile(x, 0, Tile.Wall);
            SetTile(x, Height - 1, Tile.Wall);
        }
        for (int y = 0; y < Height; y++)
        {
            SetTile(0, y, Tile.Wall);
            SetTile(Width - 1, y, Tile.Wall);
        }
    }

    public Tile GetTile(int x, int y) => _tiles[y * Width + x];

    public void SetTile(int x, int y, Tile tile) => _tiles[y * Width + x] = tile;

    public bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;

    public bool IsWalkable(int x, int y) => InBounds(x, y) && _tiles[y * Width + x].IsWalkable;
    
    public (int X, int Y) PlayerStart { get; set; } = (1, 1);
}