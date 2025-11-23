using Dungens.Dragon.DTO;
using Dungens.Dragon.DTO.Enum;
using Dungens.Dragon.Repository.Interfaces;
using Dungens.Drangon.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;

namespace Dungens.Drangon.Services.Implementations
{


    public class DungeonService : IDungeonService
    {
        private readonly ILogger<DungeonService> _logger;
        private readonly IPersistence _persistence;

        public DungeonService(ILogger<DungeonService> logger, IPersistence persistence)
        {
            _logger = logger;
            _persistence = persistence;
        }
        private Dictionary<string, Room> GetDungeon()
        {
            return new Dictionary<string, Room>
                {
                    { "r1", new Room { Id = "r1", Type = RoomType.Empty, Description = "The dungeon entrance.", Exits = new List<string> { "r2" }, Encounter = null } },
                    { "r2", new Room { Id = "r2", Type = RoomType.Enemy, Description = "A damp chamber with faint torchlight.", Exits = new List<string> { "r1", "r3" },
                        Encounter = new RoomEncounter { Enemies = new List<Enemy> { new Enemy { Name = "Goblin", Hp = 7, Ac = 12, Attack = "+3", Damage = "1d6" } } } } },
                    { "r3", new Room { Id = "r3", Type = RoomType.Treasure, Description = "A dusty alcove filled with old crates.", Exits = new List<string> { "r2", "r4" }, Encounter = null } },
                    { "r4", new Room { Id = "r4", Type = RoomType.Boss, Description = "A large stone hall. A looming boss awaits.", Exits = new List<string> { "r3" },
                        Encounter = new RoomEncounter { Enemies = new List<Enemy> { new Enemy { Name = "Orc Chieftain", Hp = 20, Ac = 14, Attack = "+4", Damage = "1d10" } } } } }
                };
        }
        public Dungeon GenerateDungeon(int? seed = null)
        {
            try
            {
                seed ??= new Random().Next();
                var rng = new Random(seed.Value);

                _logger.LogInformation("Generating dungeon with seed {Seed}", seed);

                var rooms = GetDungeon();

                var dungeon = new Dungeon
                {
                    Seed = seed.Value,
                    Rooms = rooms,
                    StartRoomId = "r1",
                    BossRoomId = "r4"
                };
                _persistence.SaveData(JsonConvert.SerializeObject(dungeon), "dungeon.json");
                _logger.LogInformation("Dungeon generated with {RoomCount} rooms.", rooms.Count);

                return dungeon;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().Name);
                if (ex is ArgumentException)
                    throw;
                else
                    throw new Exception("Error!! Please check the logs for more details");
            }
        }

        public Room GetRoom(string roomId)
        {
            try
            {
                var dungeon = _persistence.RetrieveData<Dungeon>("dungeon.json") ?? throw new Exception("No dungeon loaded");

                if (!dungeon.Rooms.TryGetValue(roomId, out var room))
                    throw new Exception($"Room {roomId} not found");

                return room;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().Name);
                if (ex is ArgumentException)
                    throw;
                else
                    throw new Exception("Error!! Please check the logs for more details");
            }
        }
    }

}
