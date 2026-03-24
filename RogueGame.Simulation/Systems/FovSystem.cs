using GoRogue.FOV;
using SadRogue.Primitives;
using RogueGame.Simulation.Core;

namespace RogueGame.Simulation.Systems;

public class FovSystem
{
    private readonly DungeonMap _map;
    private readonly IFOV _fov;
    private const int FovRadius = 8;

    public FovSystem(DungeonMap map)
    {
        _map = map;
        _fov = new RecursiveShadowcastingFOV(map.TransparencyView);
    }

    public void Calculate(int playerX, int playerY)
    {
        _map.ClearVisibility();

        _fov.Calculate(new Point(playerX, playerY), FovRadius);

        foreach (var pos in _fov.CurrentFOV)
            _map.SetVisible(pos.X, pos.Y, true);
    }
}