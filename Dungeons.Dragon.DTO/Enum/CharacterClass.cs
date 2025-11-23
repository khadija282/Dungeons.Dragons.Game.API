using System.Text.Json.Serialization;

namespace Dungens.Dragon.DTO.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CharacterClass
    {
        [JsonPropertyName("fighter")]
        Fighter,

        [JsonPropertyName("rogue")]
        Rogue,

        [JsonPropertyName("wizard")]
        Wizard
    }
}
