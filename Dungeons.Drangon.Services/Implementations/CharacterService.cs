using Dungens.Dragon.DTO;
using Dungens.Dragon.Repository.Interfaces;
using Dungens.Drangon.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Dungens.Drangon.Services.Implementations
{
    public class CharacterService : ICharacterService
    {
        private readonly ILogger<CharacterService> _logger;
        private readonly IPersistence _persistence;
        private readonly String fileName = "characters.json";
        public CharacterService(ILogger<CharacterService> logger, IPersistence persistence)
        {
            _persistence = persistence;
            _logger = logger;
        }
        public Character SaveCharacter(Character character)
        {
            try
            {
                character.Id ??= Guid.NewGuid().ToString();
                _persistence.SaveData(Newtonsoft.Json.JsonConvert.SerializeObject(character), fileName);
                return character;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().Name);
                if (ex is ArgumentException)
                    throw;
                else
                    throw new Exception("Error!! Please check the logs for more details");
            }
        }
        public Character GetCharacterById(string CharacterId)
        {
            try
            {
                var foundCharacter = _persistence.RetrieveDataById<Character>(fileName, CharacterId);
                if (foundCharacter == null)
                {
                    _logger.LogWarning("Character with ID {CharacterId} not found", CharacterId);
                    throw new ArgumentException("Character not found");
                }
                return foundCharacter;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().Name);
                if (ex is ArgumentException)
                    throw;
                else
                    throw new Exception("Error!! Please check the logs for more details");
            }
        }
        public bool DeleteCharacter(string CharacterId)
        {
            try
            {
                var run = _persistence.RetrieveData<Character>(fileName);
                if (run == null)
                {
                    _logger.LogWarning("Character with ID {CharacterId} not found", CharacterId);
                    throw new ArgumentException("Character not found");
                }
                return _persistence.RemoveDataById<Character>(fileName, CharacterId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().Name);
                if (ex is ArgumentException)
                    throw;
                else
                    throw new Exception("Error!! Please check the logs for more details");
            }
        }
    }
}
