namespace RogueGame.Simulation.Components;

public struct Glyph
{
    public char Char;
    public (byte R, byte G, byte B) Foreground;

    public Glyph(char c, (byte R, byte G, byte B) foreground)
    {
        Char = c;
        Foreground = foreground;
    }
}