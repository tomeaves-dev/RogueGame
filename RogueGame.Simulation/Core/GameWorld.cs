using Arch.Core;
using Arch.Core.Extensions;
using RogueGame.Simulation.Components;
using RogueGame.Simulation.Generation;

namespace RogueGame.Simulation.Core;

public class GameWorld
{
    private readonly World _world;

    public DungeonMap Map { get; }
    public Entity Player { get; private set; }

    public GameWorld(int mapWidth, int mapHeight)
    {
        _world = World.Create();
        Map = new DungeonMap(mapWidth, mapHeight);

        DungeonGenerator.Generate(Map);

        CreatePlayer(Map.PlayerStart.X, Map.PlayerStart.Y);
    }

    private void CreatePlayer(int x, int y)
    {
        Player = _world.Create(
            new Position(x, y),
            new Health(100),
            new Stats(10, 8, 6),
            new Glyph('@', (255, 255, 0)),
            new PlayerTag()
        );
    }

    public bool TryMove(Entity entity, int dx, int dy)
    {
        ref var pos = ref _world.Get<Position>(entity);

        int newX = pos.X + dx;
        int newY = pos.Y + dy;

        if (!Map.IsWalkable(newX, newY))
            return false;

        pos.X = newX;
        pos.Y = newY;
        return true;
    }

    public ref T GetComponent<T>(Entity entity) where T : struct
        => ref _world.Get<T>(entity);
}