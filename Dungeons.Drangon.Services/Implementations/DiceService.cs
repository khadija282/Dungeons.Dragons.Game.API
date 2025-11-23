

using Dungens.Drangon.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Dungens.Drangon.Services.Implementations
{
    public class DiceService : IDiceService
    {
        private readonly ILogger _logger;
        public DiceService(ILogger<DiceService> logger)
        {
            _logger = logger;
        }
        public int Roll(string formula, int? seed = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(formula))
                {
                    _logger.LogError("Dice formula is null or empty.");
                    throw new ArgumentException("Dice formula is required.");
                }
                _logger.LogInformation($"Rolling dice with formula: {formula} and seed: {seed}");
                var rng = seed.HasValue ? new Random(seed.Value) : new Random();
                var match = Regex.Match(formula, @"(?<count>\d*)d(?<sides>\d+)(?<modifier>[+-]\d+)?");
                if (!match.Success)
                {
                    _logger.LogError("Invalid dice formula.");
                    throw new ArgumentException("Invalid dice formula.");
                }
                _logger.LogInformation("Dice formula parsed successfully.");
                int count = string.IsNullOrEmpty(match.Groups["count"].Value) ? 1 : int.Parse(match.Groups["count"].Value);
                int sides = int.Parse(match.Groups["sides"].Value);
                int modifier = match.Groups["modifier"].Success ? int.Parse(match.Groups["modifier"].Value) : 0;

                int total = 0;
                for (int i = 0; i < count; i++)
                {
                    total += rng.Next(1, sides + 1);
                }
                _logger.LogInformation($"Dice roll result: {total} with modifier: {modifier}");
                return total + modifier;
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
