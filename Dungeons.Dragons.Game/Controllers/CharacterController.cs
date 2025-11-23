using Dungens.Dragon.DTO;
using Dungens.Drangon.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace Dungens.Dragon.Controllers
{
    [ApiController]
    [Route("characters")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;
        private readonly ILogger<CharacterController> _logger;

        public CharacterController(ICharacterService characterService, ILogger<CharacterController> logger)
        {
            _characterService = characterService;
            _logger = logger;
        }
        [HttpPost]
        public IActionResult CreateCharacter([FromBody] Character character)
        {
            try
            {
                _logger.LogInformation("Creating new character: {CharacterName}", character.Name);
                var created = _characterService.SaveCharacter(character);
                return Ok(created.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating character");
                if (ex is ArgumentException)
                    return BadRequest(ex.Message);
                else
                    return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("{id}")]
        public IActionResult GetCharacterById(string id)
        {
            try
            {
                var character = _characterService.GetCharacterById(id);
                if (character == null)
                    return NotFound($"Character with id {id} not found.");

                return Ok(character);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving character {CharacterId}", id);
                if (ex is ArgumentException)
                    return BadRequest(ex.Message);
                else
                    return StatusCode(500, "Internal server error");
            }
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteCharacter(string id)
        {
            try
            {
                bool deleted = _characterService.DeleteCharacter(id);
                if (!deleted)
                    return NotFound($"Character with id {id} not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting character {CharacterId}", id);
                if (ex is ArgumentException)
                    return BadRequest(ex.Message);
                else
                    return StatusCode(500, "Internal server error");
            }
        }
    }
}