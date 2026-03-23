using Spectre.Console;
using RogueGame.Simulation.Components;
using RogueGame.Simulation.Core;

var world = new GameWorld(80, 40);

AnsiConsole.Clear();
Console.CursorVisible = false;

void DrawGlyph(int x, int y, char glyph, (byte R, byte G, byte B) color)
{
    Console.SetCursorPosition(x, y);
    AnsiConsole.Markup($"[#{color.R:X2}{color.G:X2}{color.B:X2}]{glyph}[/]");
}

void Render()
{
    var map = world.Map;

    for (int y = 0; y < map.Height; y++)
    for (int x = 0; x < map.Width; x++)
    {
        var tile = map.GetTile(x, y);
        DrawGlyph(x, y, tile.Glyph, tile.Foreground);
    }

    var playerPos = world.GetComponent<Position>(world.Player);
    var playerGlyph = world.GetComponent<Glyph>(world.Player);
    DrawGlyph(playerPos.X, playerPos.Y, playerGlyph.Char, playerGlyph.Foreground);

    Console.SetCursorPosition(0, map.Height + 1);
    AnsiConsole.MarkupLine($"HP: [red]{world.GetComponent<Health>(world.Player).Current}[/]  " +
                           $"Position: [grey]{playerPos.X}, {playerPos.Y}[/]");
    AnsiConsole.MarkupLine("[grey]Arrow keys to move, Q to quit[/]");
}

while (true)
{
    Render();

    var key = Console.ReadKey(intercept: true).Key;

    var oldPos = world.GetComponent<Position>(world.Player);
    var oldTile = world.Map.GetTile(oldPos.X, oldPos.Y);
    DrawGlyph(oldPos.X, oldPos.Y, oldTile.Glyph, oldTile.Foreground);

    switch (key)
    {
        case ConsoleKey.UpArrow:    world.TryMove(world.Player, 0, -1); break;
        case ConsoleKey.DownArrow:  world.TryMove(world.Player, 0,  1); break;
        case ConsoleKey.LeftArrow:  world.TryMove(world.Player, -1, 0); break;
        case ConsoleKey.RightArrow: world.TryMove(world.Player,  1, 0); break;
        case ConsoleKey.Q:
            Console.CursorVisible = true;
            return;
    }
}