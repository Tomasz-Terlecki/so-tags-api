using System.Text.Json.Serialization;

namespace SoTags.DataProvider.Dtos;

/// <summary>
/// DTO for deserializing StackOverflow API response wrapper
/// </summary>
public record SoTagsResponseDto(
    [property: JsonPropertyName("items")] List<SoTagDto> Items,
    [property: JsonPropertyName("has_more")] bool HasMore,
    [property: JsonPropertyName("quota_max")] int QuotaMax = 0,
    [property: JsonPropertyName("quota_remaining")] int QuotaRemaining = 0
);
