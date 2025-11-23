using Dungens.Dragon.DTO.Enum;

namespace Dungens.Dragon.DTO
{
    public class Room
    {
        public Room()
        {
            Encounter = new RoomEncounter();
        }
        public string Id { get; set; }

        // enemy | treasure | trap | empty | boss
        public RoomType Type { get; set; }

        public string Description { get; set; }
        public List<string> Exits { get; set; }

        public RoomEncounter Encounter { get; set; }
    }

}
