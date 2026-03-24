using SadRogue.Primitives.GridViews;

namespace RogueGame.Simulation.Core;

public struct Tile
{
    public char Glyph;
    public (byte R, byte G, byte B) Foreground;
    public bool IsWalkable;
    public bool IsTransparent;
    public bool IsExplored;
    public bool IsVisible;

    public static Tile Floor => new Tile
    {
        Glyph = '.',
        Foreground = (100, 100, 100),
        IsWalkable = true,
        IsTransparent = true,
        IsExplored = false,
        IsVisible = false
    };

    public static Tile Wall => new Tile
    {
        Glyph = '#',
        Foreground = (180, 180, 180),
        IsWalkable = false,
        IsTransparent = false,
        IsExplored = false,
        IsVisible = false
    };
}

public class DungeonMap
{
    public int Width { get; }
    public int Height { get; }
    public (int X, int Y) PlayerStart { get; set; } = (1, 1);

    private readonly Tile[] _tiles;

    // GoRogue reads this to know what blocks vision
    public IGridView<bool> TransparencyView { get; }

    public DungeonMap(int width, int height)
    {
        Width = width;
        Height = height;
        _tiles = new Tile[width * height];

        TransparencyView = new LambdaGridView<bool>(
            width, height,
            pos => InBounds(pos.X, pos.Y) && _tiles[pos.Y * Width + pos.X].IsTransparent
        );
    }

    public Tile GetTile(int x, int y) => _tiles[y * Width + x];

    public void SetTile(int x, int y, Tile tile) => _tiles[y * Width + x] = tile;

    public bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;

    public bool IsWalkable(int x, int y) => InBounds(x, y) && _tiles[y * Width + x].IsWalkable;

    public void SetVisible(int x, int y, bool visible)
    {
        var tile = _tiles[y * Width + x];
        tile.IsVisible = visible;
        if (visible) tile.IsExplored = true;
        _tiles[y * Width + x] = tile;
    }

    public void ClearVisibility()
    {
        for (int i = 0; i < _tiles.Length; i++)
            _tiles[i].IsVisible = false;
    }
}