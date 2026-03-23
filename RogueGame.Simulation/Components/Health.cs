namespace RogueGame.Simulation.Components;

public struct Health
{
    public int Current;
    public int Max;

    public Health(int max)
    {
        Max = max;
        Current = max;
    }
}