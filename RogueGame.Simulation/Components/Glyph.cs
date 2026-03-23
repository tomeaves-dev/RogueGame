namespace RogueGame.Simulation.Components;


public struct Glyph
{
    public char Char;
    public (byte R, byte G, byte B) Foreground;
    public (byte R, byte G, byte B) Background;

    public Glyph(char c, (byte, byte, byte) foreground, (byte, byte, byte) background)
    {
        Char = c;
        Foreground = foreground;
        Background = background;
    }
}
