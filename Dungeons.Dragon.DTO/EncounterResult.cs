namespace Dungens.Dragon.DTO
{
    public class EncounterResult
    {
        public int RollResult { get; set; }       // Dice roll result
        public bool Hit { get; set; }             // Whether the attack/action hit
        public int Damage { get; set; }           // Damage dealt to enemy
        public int EnemyHp { get; set; }          // Enemy's remaining HP
        public int CharacterHp { get; set; }      // Character's remaining HP
        public bool EnemyDead { get; set; }       // True if enemy defeated
        public bool CharacterDead { get; set; }   // True if character defeated
    }
}
