using Dungens.Dragon.DTO.Enum;

namespace Dungens.Dragon.DTO
{
    public class EncounterAction
    {
        public ActionType Action { get; set; }
        public string? WeaponId { get; set; }
        public string? SpellId { get; set; }
    }
}
