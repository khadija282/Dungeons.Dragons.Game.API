using Dungens.Dragon.DTO;
using Dungens.Dragon.Repository.Interfaces;
using Dungens.Drangon.Services.Interfaces;
using Newtonsoft.Json;

namespace Dungeons.Dragon.Tests
{
    public class FakePersistence : IPersistence
    {
        private readonly Dictionary<string, string> _store = new();
        public T RetrieveDataById<T>(string filePath, string id)
        {
            if (!_store.ContainsKey(filePath)) return default;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(_store[filePath]);
        }
        public T RetrieveData<T>(string filePath)
        {
            if (!_store.ContainsKey(filePath)) return default;
            return JsonConvert.DeserializeObject<T>(_store[filePath]);
        }
        public bool RemoveDataById<T>(string filePath, string Id)
        {
            if (!_store.ContainsKey(filePath)) return false;
            _store.Remove(filePath);
            return true;
        }

        public bool SaveData(string data, string filePath)
        {
            _store[filePath] = data;
            return true;
        }
    }

    public class FakeCharacterService : ICharacterService
    {
        public Character _character = new() { Id = "char1", Name = "Aria", Hp = 20 };

        public bool DeleteCharacter(string CharacterId)
        {
            if (_character.Id == CharacterId)
            {
                _character = null;
                return true;
            }
            return false;
        }

        public Character GetCharacterById(string id) => id == _character.Id ? _character : null;
        public Character SaveCharacter(Character character) => _character = character;
    }

    public class FakeDungeonService : IDungeonService
    {
        public Dungeon GenerateDungeon(int? seed) => new Dungeon { StartRoomId = "r1", Seed = 123 };

        public Room GetRoom(string roomId) => new Room
        {
            Id = roomId,
            Encounter = new RoomEncounter
            {
                Enemies = new List<Enemy> { new Enemy { Name = "Goblin", Hp = 5, Ac = 1 } }
            },
            Exits = new List<string> { "r2" }
        };
    }

    public class FakeDiceService : IDiceService
    {
        private readonly int _fixedRoll;

        public FakeDiceService(int fixedRoll = 10)
        {
            _fixedRoll = fixedRoll;
        }

        public int Roll(string formula, int? seed = null)
        {
            // Always return fixed number for predictable unit tests
            return _fixedRoll;
        }
    }
}
