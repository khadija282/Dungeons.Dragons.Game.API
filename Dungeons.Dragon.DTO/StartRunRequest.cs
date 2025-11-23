namespace Dungens.Dragon.DTO
{
    public class StartRunRequest
    {
        public string CharacterId { get; set; }
        public string? Difficulty { get; set; }
        public int? Seed { get; set; }
    }
}
