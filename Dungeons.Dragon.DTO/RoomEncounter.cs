namespace Dungens.Dragon.DTO
{
    public class RoomEncounter
    {
        public RoomEncounter()
        {
            Enemies = new List<Enemy>();
        }
        public List<Enemy> Enemies { get; set; }
    }
}
