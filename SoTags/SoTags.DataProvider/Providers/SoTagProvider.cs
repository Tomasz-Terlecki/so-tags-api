using System.Text.Json.Serialization;
using SoTags.Domain.Interfaces.DataProviders;
using SoTags.Domain.Models;

namespace SoTags.DataProvider.Providers;

/// <summary>
/// DTO for deserializing StackOverflow API tag response
/// </summary>
internal record SoTagDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("count")] int Count,
    [property: JsonPropertyName("has_synonyms")] bool HasSynonyms
);

/// <summary>
/// DTO for deserializing StackOverflow API response wrapper
/// </summary>
internal record SoTagsResponseDto(
    [property: JsonPropertyName("items")] List<SoTagDto> Items,
    [property: JsonPropertyName("has_more")] bool HasMore,
    [property: JsonPropertyName("quota_max")] int QuotaMax,
    [property: JsonPropertyName("quota_remaining")] int QuotaRemaining
);

/// <summary>
/// Provides functionality to download tags from StackOverflow
/// </summary>
public class SoTagProvider : ISoTagProvider
{
    private readonly HttpClient _httpClient;
    private const string StackOverflowApiBaseUrl = "https://api.stackexchange.com/2.3";
    private const int PageSize = 100;

    public SoTagProvider(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
    

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        // Implementation for getting the total count of tags
        var url = $"{StackOverflowApiBaseUrl}/tags" +
                  $"?pagesize=1" +
                  $"&page=1" +
                  $"&order=desc" +
                  $"&sort=popular" +
                  $"&site=stackoverflow";

        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var tagsResponse = System.Text.Json.JsonSerializer.Deserialize<SoTagsResponseDto>(content, options);
            return tagsResponse?.QuotaMax ?? 0; // Using QuotaMax as an approximation for total count
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Failed to fetch tags from StackOverflow API: {ex.Message}", ex);
        }
        catch (System.Text.Json.JsonException ex)
        {
            throw new InvalidOperationException($"Failed to deserialize StackOverflow API response: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<SoTag>> GetAsync(int count, CancellationToken cancellationToken = default)
    {
        if (count <= 0)
            throw new ArgumentException("Count must be greater than 0", nameof(count));

        var tags = new List<SoTag>();
        var totalPages = (count + PageSize - 1) / PageSize;

        for (int page = 1; page <= totalPages && tags.Count < count; page++)
        {
            var response = await FetchTagsPageAsync(page, cancellationToken);

            if (response?.Items == null || response.Items.Count == 0)
                break;

            foreach (var tagDto in response.Items)
            {
                if (tags.Count >= count)
                    break;

                var soTag = new SoTag(
                    Id: Guid.NewGuid(),
                    Name: tagDto.Name,
                    Description: string.Empty
                );
                tags.Add(soTag);
            }

            // If there are no more pages, stop
            if (!response.HasMore)
                break;
        }

        return tags;
    }

    /// <summary>
    /// Fetches a single page of tags from the StackOverflow API
    /// </summary>
    private async Task<SoTagsResponseDto?> FetchTagsPageAsync(int page, CancellationToken cancellationToken)
    {
        var url = $"{StackOverflowApiBaseUrl}/tags" +
                  $"?pagesize={PageSize}" +
                  $"&page={page}" +
                  $"&order=desc" +
                  $"&sort=popular" +
                  $"&site=stackoverflow";

        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return System.Text.Json.JsonSerializer.Deserialize<SoTagsResponseDto>(content, options);
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Failed to fetch tags from StackOverflow API: {ex.Message}", ex);
        }
        catch (System.Text.Json.JsonException ex)
        {
            throw new InvalidOperationException($"Failed to deserialize StackOverflow API response: {ex.Message}", ex);
        }
    }

    public Task<IEnumerable<SoTag>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
