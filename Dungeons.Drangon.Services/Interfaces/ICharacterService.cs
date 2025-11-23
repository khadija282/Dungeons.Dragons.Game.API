using Dungens.Dragon.DTO;

namespace Dungens.Drangon.Services.Interfaces
{
    public interface ICharacterService
    {
        public Character SaveCharacter(Character character);
        bool DeleteCharacter(string CharacterId);
        Character GetCharacterById(string CharacterId);
    }
}