using Spectre.Console;

int mapWidth = 40;
int mapHeight = 20;
int playerX = mapWidth / 2;
int playerY = mapHeight / 2;

Console.CursorVisible = false;

void DrawGlyph(int x, int y, char glyph, Color color)
{
    Console.SetCursorPosition(x, y);
    AnsiConsole.Markup($"[{color}]{glyph}[/]");
}

void Render()
{
    // Draw floor
    for (int y = 1; y < mapHeight - 1; y++)
    for (int x = 1; x < mapWidth - 1; x++)
        DrawGlyph(x, y, '.', Color.Grey23);
    
    // Draw walls
    for (int x = 0; x < mapWidth; x++)
    {
        DrawGlyph(x, 0, '#', Color.Grey);              // top wall
        DrawGlyph(x, mapHeight - 1, '#', Color.Grey);  // bottom wall
    }
    for (int y = 0; y < mapHeight; y++)
    {
        DrawGlyph(0, y, '#', Color.Grey);              // left wall
        DrawGlyph(mapWidth - 1, y, '#', Color.Grey);   // right wall
    }

    // Draw player
    DrawGlyph(playerX, playerY, '@', Color.Yellow);

    // Draw HUD below map
    Console.SetCursorPosition(0, mapHeight + 1);
    AnsiConsole.MarkupLine($"Position: [grey]{playerX}, {playerY}[/]");
    AnsiConsole.MarkupLine("[grey]Arrow keys to move, Q to quit[/]");
}

while (true)
{
    Render();

    var key = Console.ReadKey(intercept: true).Key;

    DrawGlyph(playerX, playerY, '.', Color.Grey23);

    switch (key)
    {
        case ConsoleKey.UpArrow:    playerY--; break;
        case ConsoleKey.DownArrow:  playerY++; break;
        case ConsoleKey.LeftArrow:  playerX--; break;
        case ConsoleKey.RightArrow: playerX++; break;
        case ConsoleKey.Q:
            Console.CursorVisible = true;
            return;
    }

    playerX = Math.Clamp(playerX, 1, mapWidth - 2);
    playerY = Math.Clamp(playerY, 1, mapHeight - 2);
}