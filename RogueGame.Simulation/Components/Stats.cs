namespace RogueGame.Simulation.Components;

public struct Stats
{
    public int Strength;
    public int Dexterity;
    public int Intelligence;

    public Stats(int strength, int dexterity, int intelligence)
    {
        Strength = strength;
        Dexterity = dexterity;
        Intelligence = intelligence;
    }
}