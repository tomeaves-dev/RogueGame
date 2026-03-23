using GoRogue.MapGeneration;
using GoRogue.MapGeneration.Steps;
using GoRogue.MapGeneration.Steps.Translation;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using RogueGame.Simulation.Core;

namespace RogueGame.Simulation.Generation;

public static class DungeonGenerator
{
    public static void Generate(
        DungeonMap map,
        int minRooms = 5,
        int maxRooms = 10,
        int minRoomSize = 4,
        int maxRoomSize = 10)
    {
        var generator = new Generator(map.Width, map.Height);

        generator.ConfigAndGenerateSafe(gen =>
        {
            gen
                .AddStep(new RoomsGeneration
                {
                    MinRooms = minRooms,
                    MaxRooms = maxRooms,
                    RoomMinSize = minRoomSize,
                    RoomMaxSize = maxRoomSize
                })
                .AddStep(new RectanglesToAreas("Rooms", "Areas"))
                .AddStep(new ClosestMapAreaConnection());
        });

        // Get wall/floor data — true = floor, false = wall
        var wallFloor = generator.Context.GetFirst<ISettableGridView<bool>>("WallFloor");

        // Translate into our tile format
        foreach (var pos in wallFloor.Positions())
            map.SetTile(pos.X, pos.Y, wallFloor[pos] ? Tile.Floor : Tile.Wall);

        // Place player in centre of first room
        var rooms = generator.Context
            .GetFirst<GoRogue.MapGeneration.ContextComponents.ItemList<Rectangle>>("Rooms");

        if (rooms != null && rooms.Count() > 0)
        {
            var center = rooms.Items[0].Center;
            map.PlayerStart = (center.X, center.Y);
        }
    }
}