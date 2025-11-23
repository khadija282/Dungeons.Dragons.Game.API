using System.Text.Json.Serialization;

namespace Dungens.Dragon.DTO.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ActionType
    {
        [JsonPropertyName("attack")]
        Attack,
        [JsonPropertyName("defend")]
        Defend,
        [JsonPropertyName("cast")]
        Cast
    }
}
