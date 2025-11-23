using Dungens.Dragon.DTO;
using Dungens.Dragon.DTO.Enum;
using Dungens.Dragon.Repository.Implementations;
using Dungens.Drangon.Services.Implementations;
using Microsoft.Extensions.Logging;
using Xunit;

public class RunServiceTests
{
    private readonly RunService _runService;

    public RunServiceTests()
    {
        var loggerFactory = new LoggerFactory();
        var logger = loggerFactory.CreateLogger<RunService>();

        var persistence = new FilePersistence(loggerFactory.CreateLogger<FilePersistence>());

        var characterService = new CharacterService(loggerFactory.CreateLogger<CharacterService>(), persistence);
        var dungeonService = new DungeonService(loggerFactory.CreateLogger<DungeonService>(), persistence);
        var diceService = new DiceService(loggerFactory.CreateLogger<DiceService>());

        _runService = new RunService(logger, dungeonService, characterService, persistence, diceService);
    }

    [Fact]
    public void StartRun_ShouldCreateRun()
    {
        var run = _runService.StartRun("char1");

        Assert.NotNull(run);
        Assert.Equal("char1", run.CharacterId);
        Assert.Equal("r1", run.CurrentRoomId);
    }

    [Fact]
    public void StartRun_InvalidCharacter_ShouldThrow()
    {
        var ex = Assert.Throws<Exception>(() => _runService.StartRun("invalid"));
        Assert.Equal("Character not found", ex.Message);
    }

    [Fact]
    public void AbortRun_ShouldSetStatusAborted()
    {
        var run = _runService.StartRun("char1");
        var abortedRun = _runService.AbortRun(run.Id);

        Assert.Equal("aborted", abortedRun.Status);
        Assert.Contains(abortedRun.Log, l => l.Event.Contains("aborted"));
    }

    [Fact]
    public void RollEncounter_ShouldReturnResult()
    {
        var run = _runService.StartRun("char1");
        var action = new EncounterAction { Action = ActionType.Attack };

        var result = _runService.RollEncounter(run.Id, action);

        Assert.NotNull(result);
        Assert.True(result.CharacterHp > 0);
    }
}
