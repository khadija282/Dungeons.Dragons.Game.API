using System.Text.Json.Serialization;

namespace Dungens.Dragon.DTO.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RunStatus
    {
        [JsonPropertyName("in_progress")]
        InProgress,

        [JsonPropertyName("defeated")]
        Defeated,

        [JsonPropertyName("victory")]
        Victory,

        [JsonPropertyName("aborted")]
        Aborted
    }
}
