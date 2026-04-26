using System.Text.Json.Serialization;

namespace SoTags.DataProvider.Dtos;

/// <summary>
/// DTO for deserializing StackOverflow API tag response
/// </summary>
internal record SoTagDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("count")] int Count,
    [property: JsonPropertyName("has_synonyms")] bool HasSynonyms,
    [property: JsonPropertyName("is_moderator_only")] bool IsModeratorOnly,
    [property: JsonPropertyName("is_required")] bool IsRequired
);
