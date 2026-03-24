using Spectre.Console;
using RogueGame.Simulation.Components;
using RogueGame.Simulation.Core;
using RogueGame.Console.Rendering;

var world = new GameWorld(120, 80);
var viewport = new Viewport(60, 40);

AnsiConsole.Clear();
Console.CursorVisible = false;

void DrawGlyph(int screenX, int screenY, char glyph, (byte R, byte G, byte B) color)
{
    Console.SetCursorPosition(screenX, screenY);
    AnsiConsole.Markup($"[#{color.R:X2}{color.G:X2}{color.B:X2}]{glyph}[/]");
}

void Render()
{
    var map = world.Map;
    var playerPos = world.GetComponent<Position>(world.Player);

    // Re-centre viewport on player each frame
    viewport.Center(playerPos.X, playerPos.Y, map.Width, map.Height);

    // Draw only tiles within the viewport
    for (int y = 0; y < viewport.Height; y++)
    for (int x = 0; x < viewport.Width; x++)
    {
        int mapX = viewport.X + x;
        int mapY = viewport.Y + y;

        var tile = map.GetTile(mapX, mapY);

        if (!tile.IsExplored)
            DrawGlyph(x, y, ' ', (0, 0, 0));
        else if (!tile.IsVisible)
            DrawGlyph(x, y, tile.Glyph, (40, 40, 40));
        else
            DrawGlyph(x, y, tile.Glyph, tile.Foreground);
    }

    // Draw player at screen position
    var (screenX, screenY) = viewport.ToScreen(playerPos.X, playerPos.Y);
    var playerGlyph = world.GetComponent<Glyph>(world.Player);
    DrawGlyph(screenX, screenY, playerGlyph.Char, playerGlyph.Foreground);

    // HUD below viewport
    Console.SetCursorPosition(0, viewport.Height + 1);
    AnsiConsole.MarkupLine($"HP: [red]{world.GetComponent<Health>(world.Player).Current}[/]  " +
                           $"Position: [grey]{playerPos.X}, {playerPos.Y}[/]");

    var status = world.IsAutoExploring
        ? "[yellow]Auto-exploring...[/]"
        : "[grey]O: auto-explore  Q: quit[/]";
    AnsiConsole.MarkupLine(status);
}

void ErasePlayer()
{
    var playerPos = world.GetComponent<Position>(world.Player);
    var (screenX, screenY) = viewport.ToScreen(playerPos.X, playerPos.Y);
    var tile = world.Map.GetTile(playerPos.X, playerPos.Y);

    if (!tile.IsVisible)
        DrawGlyph(screenX, screenY, tile.Glyph, (40, 40, 40));
    else
        DrawGlyph(screenX, screenY, tile.Glyph, tile.Foreground);
}

while (true)
{
    Render();

    if (world.IsAutoExploring)
    {
        if (Console.KeyAvailable)
        {
            Console.ReadKey(intercept: true);
            world.CancelAutoExplore();
            continue;
        }

        Thread.Sleep(50);
        ErasePlayer();
        world.StepAutoExplore();
        continue;
    }

    var key = Console.ReadKey(intercept: true).Key;
    ErasePlayer();

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