namespace Dungens.Dragon.DTO
{
    public class Dungeon
    {
        public int Seed { get; set; }
        public Dictionary<string, Room> Rooms { get; set; }
        public string StartRoomId { get; set; }
        public string BossRoomId { get; set; }
    }

}
