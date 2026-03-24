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

        if (!tile.IsExplored)
            DrawGlyph(x, y, ' ', (0, 0, 0));
        else if (!tile.IsVisible)
            DrawGlyph(x, y, tile.Glyph, (40, 40, 40));
        else
            DrawGlyph(x, y, tile.Glyph, tile.Foreground);
    }

    var playerPos = world.GetComponent<Position>(world.Player);
    var playerGlyph = world.GetComponent<Glyph>(world.Player);
    DrawGlyph(playerPos.X, playerPos.Y, playerGlyph.Char, playerGlyph.Foreground);

    Console.SetCursorPosition(0, map.Height + 1);
    AnsiConsole.MarkupLine($"HP: [red]{world.GetComponent<Health>(world.Player).Current}[/]  " +
                           $"Position: [grey]{playerPos.X}, {playerPos.Y}[/]");

    var status = world.IsAutoExploring ? "[yellow]Auto-exploring...[/]" : "[grey]O: auto-explore  Q: quit[/]";
    AnsiConsole.MarkupLine(status);
}

while (true)
{
    Render();

    if (world.IsAutoExploring)
    {
        // Cancel if any key is waiting
        if (Console.KeyAvailable)
        {
            Console.ReadKey(intercept: true);
            world.CancelAutoExplore();
            continue;
        }

        Thread.Sleep(50); // Small delay so movement is visible
        world.StepAutoExplore();
        continue;
    }

    var key = Console.ReadKey(intercept: true).Key;

    var oldPos = world.GetComponent<Position>(world.Player);
    var oldTile = world.Map.GetTile(oldPos.X, oldPos.Y);
    DrawGlyph(oldPos.X, oldPos.Y, oldTile.Glyph, oldTile.Foreground);

    switch (key)
    {
        case ConsoleKey.K: world.TryMove(world.Player, 0, -1); break;
        case ConsoleKey.J: world.TryMove(world.Player, 0,  1); break;
        case ConsoleKey.H: world.TryMove(world.Player, -1, 0); break;
        case ConsoleKey.L: world.TryMove(world.Player,  1, 0); break;
        case ConsoleKey.Y: world.TryMove(world.Player, -1, -1); break;
        case ConsoleKey.U: world.TryMove(world.Player,  1, -1); break;
        case ConsoleKey.B: world.TryMove(world.Player, -1,  1); break;
        case ConsoleKey.N: world.TryMove(world.Player,  1,  1); break;
        case ConsoleKey.O: world.StartAutoExplore(); break;
        case ConsoleKey.Q:
            Console.CursorVisible = true;
            return;
    }
}