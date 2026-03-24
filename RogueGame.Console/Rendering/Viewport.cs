namespace RogueGame.Console.Rendering;

public class Viewport
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; }
    public int Height { get; }

    public Viewport(int width, int height)
    {
        Width = width;
        Height = height;
    }
    
    public void Center(int targetX, int targetY, int mapWidth, int mapHeight)
    {
        X = Math.Clamp(targetX - Width / 2, 0, mapWidth - Width);
        Y = Math.Clamp(targetY - Height / 2, 0, mapHeight - Height);
    }
    
    // Convert map coordinates to screen coordinates
    public (int ScreenX, int ScreenY) ToScreen(int mapX, int mapY)
        => (mapX - X, mapY - Y);
    
    // Check if a map position is within the viewport
    public bool Contains(int mapX, int mapY)
        => mapX >= X && mapX < X + Width &&
           mapY >= Y && mapY < Y + Height;
}