using Dungens.Dragon.DTO;

namespace Dungens.Drangon.Services.Interfaces
{
    public interface IRunService
    {
        void ApplyEncounterResults(string runId, int hpDelta, List<InventoryItem>? items = null);
        Run MoveToRoom(string runId, string roomId);
        public Run StartRun(string characterId, string? difficulty = null, int? seed = null);
        public bool IsAdjacent(string currentRoomId, string nextRoomId);
        public Run GetRunById(string runId);
        EncounterResult RollEncounter(string runId, EncounterAction action);
        Run ExploreRoom(string runId, string nextRoomId);
        public Run AbortRun(string runId);
        public Run FleeRun(string runId);
    }
}