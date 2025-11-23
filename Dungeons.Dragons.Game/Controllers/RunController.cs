using Dungens.Dragon.DTO;
using Dungens.Drangon.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dungens.Dragons.Game.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RunController : ControllerBase
    {
        private readonly IRunService _runService;
        private readonly ILogger<RunController> _logger;

        public RunController(IRunService runService, ILogger<RunController> logger)
        {
            _runService = runService;
            _logger = logger;
        }
        [HttpPost]
        public IActionResult StartRun([FromBody] StartRunRequest request)
        {
            try
            {
                var run = _runService.StartRun(request.CharacterId, request.Difficulty, request.Seed);
                return Ok(run);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting run for character {CharacterId}", request.CharacterId);
                if (ex is ArgumentException)
                    return BadRequest(ex.Message);
                else
                    return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("{id}")]
        public IActionResult GetRunById(string id)
        {
            try
            {
                var run = _runService.GetRunById(id);
                if (run == null)
                    return NotFound($"Run with id {id} not found.");

                return Ok(run);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving run {RunId}", id);
                if (ex is ArgumentException)
                    return BadRequest(ex.Message);
                else
                    return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost("runs/{id}/explore")]
        public async Task<IActionResult> Explore(string id, [FromBody] string nextRoomId)
        {
            try
            {
                _logger.LogInformation("Exploration started for {RunId}", id);
                var result = _runService.ExploreRoom(id, nextRoomId);
                _logger.LogInformation("Exploration completed successfully for run {RunId}", id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during encounter roll for run {RunId}", id);
                if (ex is ArgumentException)
                    return BadRequest(ex.Message);
                else
                    return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost("runs/{id}/encounter/roll")]
        public async Task<IActionResult> RollEncounter(string id, [FromBody] EncounterAction action)
        {
            try
            {
                _logger.LogInformation("Encounter roll started for {RunId}", id);
                var result = _runService.RollEncounter(id, action);
                _logger.LogInformation("Encounter rolled successfully for run {RunId}", id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during encounter roll for run {RunId}", id);
                if (ex is ArgumentException)
                    return BadRequest(ex.Message);
                else
                    return StatusCode(500, "Internal server error");
            }

        }
        [HttpPost("{id}/flee")]
        public IActionResult Flee(string id)
        {
            try
            {
                var result = _runService.FleeRun(id);
                return Ok(result); // return run state or message
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fleeing run {RunId}", id);
                if (ex is ArgumentException)
                    return BadRequest(ex.Message);
                else
                    return StatusCode(500, "Internal server error");
            }
        }

        // --- Abort ---
        [HttpPost("{id}/abort")]
        public IActionResult Abort(string id)
        {
            try
            {
                var run = _runService.AbortRun(id);
                return Ok(run);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error aborting run {RunId}", id);
                if (ex is ArgumentException)
                    return BadRequest(ex.Message);
                else
                    return StatusCode(500, "Internal server error");
            }
        }


    }
}
