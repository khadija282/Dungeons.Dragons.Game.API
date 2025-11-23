using Dungens.Dragon.DTO.Enum;

namespace Dungens.Dragon.DTO
{
    public class Character
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public CharacterClass Class { get; set; }
        public int Level { get; set; }
        public int Hp { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }

        public List<InventoryItem> Inventory { get; set; }
    }
}
