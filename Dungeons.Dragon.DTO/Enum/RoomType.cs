using System.Text.Json.Serialization;

namespace Dungens.Dragon.DTO.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RoomType
    {
        [JsonPropertyName("enemy")]
        Enemy,

        [JsonPropertyName("treasure")]
        Treasure,

        [JsonPropertyName("trap")]
        Trap,

        [JsonPropertyName("empty")]
        Empty,

        [JsonPropertyName("boss")]
        Boss
    }
}
