using Dungens.Dragon.DTO;

namespace Dungens.Drangon.Services.Interfaces
{
    public interface IDungeonService
    {
        Dungeon GenerateDungeon(int? seed = null);
        Room GetRoom(string roomId);
    }
}
