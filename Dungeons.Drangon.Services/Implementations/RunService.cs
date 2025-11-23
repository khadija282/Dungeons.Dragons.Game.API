using Dungens.Dragon.DTO;
using Dungens.Dragon.DTO.Enum;
using Dungens.Dragon.Repository.Interfaces;
using Dungens.Drangon.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;
namespace Dungens.Drangon.Services.Implementations
{
    public class RunService : IRunService
    {
        private readonly ILogger<RunService> _logger;
        private readonly IDungeonService _dungeonService;
        private readonly ICharacterService _characterService;
        private readonly IDiceService _diceService;
        private readonly IPersistence _persistence;
        public RunService(ILogger<RunService> logger, IDungeonService dungeonService, ICharacterService characterService, IPersistence persistence, IDiceService diceService)
        {
            _logger = logger;
            _dungeonService = dungeonService;
            _characterService = characterService;
            _diceService = diceService;
            _persistence = persistence;
        }
        public Run StartRun(string characterId, string? difficulty = null, int? seed = null)
        {
            try
            {
                _logger.LogInformation("Starting new run for character {CharacterId}", characterId);

                var character = _characterService.GetCharacterById(characterId);
                if (character == null)
                {
                    _logger.LogWarning("Character {CharacterId} not found", characterId);
                    throw new ArgumentException("Character not found");
                }
                var dungeon = _dungeonService.GenerateDungeon(seed);
                var startRoom = dungeon.StartRoomId;

                var run = new Run
                {
                    Id = Guid.NewGuid().ToString(),
                    CharacterId = characterId,
                    StartedAt = DateTime.UtcNow,
                    Status = RunStatus.InProgress.ToString().ToLower(),
                    CurrentRoomId = startRoom,
                    DiscoveredRoomIds = new List<string> { startRoom },
                    Difficulty = difficulty,
                    Seed = dungeon.Seed,
                    Log = new List<RunLog>
            {
                new RunLog { Ts = DateTime.UtcNow.ToString("o"), Event = $"Run started in room {startRoom}" }
            }
                };

                _persistence.SaveData(Newtonsoft.Json.JsonConvert.SerializeObject(run), $"run_{run.Id}.json");

                _logger.LogInformation("Run {RunId} started successfully", run.Id);

                return run;
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
        public Run GetRunById(string runId)
        {
            try
            {
                var run = _persistence.RetrieveDataById<Run>($"run_{runId}.json", runId);
                if (run == null)
                {
                    _logger.LogWarning("Run {RunId} not found", runId);
                    throw new ArgumentException("Run not found");
                }
                return run;
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
        public Run MoveToRoom(string runId, string roomId)
        {
            try
            {
                var run = _persistence.RetrieveDataById<Run>($"run_{runId}.json", runId) ?? throw new ArgumentException("Run not found");

                if (!run.DiscoveredRoomIds.Contains(roomId))
                    run.DiscoveredRoomIds.Add(roomId);

                run.CurrentRoomId = roomId;

                run.Log.Add(new RunLog
                {
                    Ts = DateTime.UtcNow.ToString("o"),
                    Event = $"Entered room {roomId}"
                });

                _persistence.SaveData(Newtonsoft.Json.JsonConvert.SerializeObject(run), $"run_{run.Id}.json");

                _logger.LogInformation("Run {RunId} moved to room {RoomId}", runId, roomId);

                return run;
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
        public void ApplyEncounterResults(string runId, int hpDelta, List<InventoryItem>? items = null)
        {
            try
            {
                var run = _persistence.RetrieveDataById<Run>($"run_{runId}.json", runId) ?? throw new ArgumentException("Run not found");

                var character = _characterService.GetCharacterById(run.CharacterId) ?? throw new ArgumentException("Character not found");

                character.Hp += hpDelta;

                if (items != null)
                {
                    foreach (var item in items)
                        character.Inventory.Add(item);
                }
                if (character.Hp <= 0)
                    run.Status = RunStatus.Defeated.ToString().ToLower();

                _characterService.SaveCharacter(character);
                _persistence.SaveData(Newtonsoft.Json.JsonConvert.SerializeObject(run), $"run_{run.Id}.json");
                _logger.LogInformation("Run {RunId}: applied encounter results (HP change: {HpDelta}, items: {ItemCount})", run.Id, hpDelta, items?.Count ?? 0);
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
        public bool IsAdjacent(string currentRoomId, string nextRoomId)
        {
            try
            {
                var currentRoom = _dungeonService.GetRoom(currentRoomId);
                return currentRoom.Exits.Contains(nextRoomId);
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
        public EncounterResult RollEncounter(string runId, EncounterAction action)
        {
            try
            {
                var run = GetRunById(runId);
                var character = _characterService.GetCharacterById(run.CharacterId) ?? throw new ArgumentException("Character not found");

                var room = _dungeonService.GetRoom(run.CurrentRoomId);
                if (room.Encounter?.Enemies == null || room.Encounter.Enemies.Count == 0)
                    throw new ArgumentException("No enemies in this room.");

                var enemy = room.Encounter.Enemies.First();
                var formula = GetFormula(action.Action);
                int rollResult = _diceService.Roll(formula, run.Seed);

                bool hit = false;
                int damage = 0;
                if (action.Action == ActionType.Attack || action.Action == ActionType.Cast)
                {
                    hit = rollResult >= enemy.Ac;
                    if (hit)
                    {
                        damage = _diceService.Roll("1d6+1", run.Seed);
                        enemy.Hp -= damage;
                    }
                }
                if (enemy.Hp > 0)
                {
                    int enemyRoll = _diceService.Roll("1d20+2", run.Seed);
                    if (enemyRoll >= character.Hp)
                        character.Hp -= 5;
                }
                run.Log.Add(new RunLog
                {
                    Ts = DateTime.UtcNow.ToString("o"),
                    Event = $"{character.Name} used {action.Action} {(hit ? $"and hit {enemy.Name} for {damage}" : "but missed")}."
                });
                if (enemy.Hp <= 0)
                {
                    run.Log.Add(new RunLog
                    {
                        Ts = DateTime.UtcNow.ToString("o"),
                        Event = $"Enemy {enemy.Name} defeated!"
                    });
                }
                if (character.Hp <= 0)
                {
                    run.Status = RunStatus.Defeated.ToString().ToLower();
                }
                _characterService.SaveCharacter(character);
                _persistence.SaveData(JsonConvert.SerializeObject(run), $"run_{run.Id}.json");

                _logger.LogInformation("Run {RunId}: encounter resolved (Roll={Roll}, Hit={Hit}, Damage={Damage}, EnemyHp={EnemyHp}, CharacterHp={CharacterHp})",
                    run.Id, rollResult, hit, damage, enemy.Hp, character.Hp);

                return new EncounterResult
                {
                    RollResult = rollResult,
                    Hit = hit,
                    Damage = damage,
                    EnemyHp = enemy.Hp,
                    CharacterHp = character.Hp,
                    EnemyDead = enemy.Hp <= 0,
                    CharacterDead = character.Hp <= 0
                };
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
        public Run ExploreRoom(string runId, string nextRoomId)
        {
            try
            {
                var run = GetRunById(runId);
                if (!IsAdjacent(run.CurrentRoomId, nextRoomId))
                    throw new Exception("The room is not adjacent.");

                if (!run.DiscoveredRoomIds.Contains(nextRoomId))
                    run.DiscoveredRoomIds.Add(nextRoomId);

                run.CurrentRoomId = nextRoomId;

                run.Log.Add(new RunLog
                {
                    Ts = DateTime.UtcNow.ToString("o"),
                    Event = $"Entered room {nextRoomId}"
                });
                _persistence.SaveData(JsonConvert.SerializeObject(run), $"run_{run.Id}.json");

                _logger.LogInformation("Run {RunId} moved to room {RoomId}", run.Id, nextRoomId);

                return run;
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
        public Run FleeRun(string runId)
        {
            try
            {
                var run = GetRunById(runId);
                var character = _characterService.GetCharacterById(run.CharacterId)
                                ?? throw new Exception("Character not found");

                var room = _dungeonService.GetRoom(run.CurrentRoomId);
                if (room.Encounter?.Enemies == null || !room.Encounter.Enemies.Any())
                    throw new Exception("No enemies to flee from.");

                bool fled = _diceService.Roll("1d2", run.Seed) == 1;

                run.Log.Add(new RunLog
                {
                    Ts = DateTime.UtcNow.ToString("o"),
                    Event = fled ? $"{character.Name} successfully fled from encounter." :
                                  $"{character.Name} failed to flee."
                });

                if (fled)
                {
                    // Optionally move to previous room or reset encounter
                    run.Status = RunStatus.InProgress.ToString().ToLower();
                    room.Encounter.Enemies.Clear(); // remove enemies for simplicity
                }

                _persistence.SaveData(JsonConvert.SerializeObject(run), $"run_{run.Id}.json");
                return run;
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
        public Run AbortRun(string runId)
        {
            try
            {
                var run = GetRunById(runId);
                run.Status = RunStatus.Aborted.ToString().ToLower();

                run.Log.Add(new RunLog
                {
                    Ts = DateTime.UtcNow.ToString("o"),
                    Event = "Run was aborted by the player."
                });

                _persistence.SaveData(JsonConvert.SerializeObject(run), $"run_{run.Id}.json");
                _logger.LogInformation("Run {RunId} aborted.", run.Id);

                return run;
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
        private string GetFormula(ActionType action)
        {
            string formula = string.Empty;
            switch (action)
            {
                case ActionType.Attack:
                    formula = "1d20+3";
                    break;
                case ActionType.Cast:
                    formula = "1d20+character.Intelligence";
                    break;
                case ActionType.Defend:
                    formula = "1d20";
                    break;
                default:
                    throw new ArgumentException("Invalid action");
            }
            return formula;
        }

    }

}
