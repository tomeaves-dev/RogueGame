using Arch.Core;
using Arch.Core.Extensions;
using GoRogue.Pathing;
using SadRogue.Primitives;
using RogueGame.Simulation.Components;
using RogueGame.Simulation.Generation;
using RogueGame.Simulation.Systems;

namespace RogueGame.Simulation.Core;

public class GameWorld
{
    private readonly World _world;
    private readonly FovSystem _fovSystem;
    private readonly PathfindingSystem _pathfindingSystem;

    private List<Point> _autoExplorePath = new();

    public DungeonMap Map { get; }
    public Entity Player { get; private set; }
    public bool IsAutoExploring { get; private set; }

    public GameWorld(int mapWidth, int mapHeight)
    {
        _world = World.Create();
        Map = new DungeonMap(mapWidth, mapHeight);

        DungeonGenerator.Generate(Map);

        _fovSystem = new FovSystem(Map);
        _pathfindingSystem = new PathfindingSystem(Map);

        CreatePlayer(Map.PlayerStart.X, Map.PlayerStart.Y);

        var startPos = Map.PlayerStart;
        _fovSystem.Calculate(startPos.X, startPos.Y);
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

        _fovSystem.Calculate(pos.X, pos.Y);

        return true;
    }

    public void StartAutoExplore()
    {
        var target = FindNearestUnexplored();
        if (target == null)
        {
            IsAutoExploring = false;
            return;
        }

        var playerPos = _world.Get<Position>(Player);
        var start = new Point(playerPos.X, playerPos.Y);

        _autoExplorePath = _pathfindingSystem.FindPath(start, target.Value);
        IsAutoExploring = _autoExplorePath.Count > 0;
    }

    // Execute one step of auto-explore, returns false if done
    public bool StepAutoExplore()
    {
        if (!IsAutoExploring || _autoExplorePath.Count == 0)
        {
            IsAutoExploring = false;
            return false;
        }

        var next = _autoExplorePath[0];
        _autoExplorePath.RemoveAt(0);

        var playerPos = _world.Get<Position>(Player);
        int dx = next.X - playerPos.X;
        int dy = next.Y - playerPos.Y;

        TryMove(Player, dx, dy);

        // Check if we've reached the end of the path
        if (_autoExplorePath.Count == 0)
            StartAutoExplore(); // Find next unexplored target

        return IsAutoExploring;
    }

    public void CancelAutoExplore() => IsAutoExploring = false;

    private Point? FindNearestUnexplored()
    {
        var playerPos = _world.Get<Position>(Player);
        Point? nearest = null;
        float nearestDist = float.MaxValue;

        for (int y = 0; y < Map.Height; y++)
        for (int x = 0; x < Map.Width; x++)
        {
            var tile = Map.GetTile(x, y);
            if (tile.IsExplored || !tile.IsWalkable) continue;

            float dist = MathF.Sqrt(
                MathF.Pow(x - playerPos.X, 2) +
                MathF.Pow(y - playerPos.Y, 2)
            );

            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = new Point(x, y);
            }
        }

        return nearest;
    }

    public ref T GetComponent<T>(Entity entity) where T : struct
        => ref _world.Get<T>(entity);
}