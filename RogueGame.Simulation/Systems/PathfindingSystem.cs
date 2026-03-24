using GoRogue.Pathing;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using RogueGame.Simulation.Core;

namespace RogueGame.Simulation.Systems;

public class PathfindingSystem
{
    private readonly DungeonMap _map;
    private readonly AStar _aStar;

    public PathfindingSystem(DungeonMap map)
    {
        _map = map;

        var walkabilityView = new LambdaGridView<bool>(
            map.Width, map.Height,
            pos => map.IsWalkable(pos.X, pos.Y)
        );

        _aStar = new AStar(walkabilityView, Distance.Chebyshev);
    }

    // Returns ordered list of steps from start to goal, excluding the start position
    public List<Point> FindPath(Point start, Point goal)
    {
        var path = _aStar.ShortestPath(start, goal);

        if (path == null)
            return new List<Point>();

        return path.Steps.ToList();
    }
}