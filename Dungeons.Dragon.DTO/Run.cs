namespace Dungens.Dragon.DTO
{
    public class Run
    {
        public string Id { get; set; }
        public string CharacterId { get; set; }
        public DateTime StartedAt { get; set; }
        public string? Difficulty { get; set; }
        public string Status { get; set; }

        public string CurrentRoomId { get; set; }
        public List<string> DiscoveredRoomIds { get; set; }

        public int Seed { get; set; }

        public List<RunLog> Log { get; set; }
    }
}
