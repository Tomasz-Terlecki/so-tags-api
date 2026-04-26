using SoTags.DataProvider.Dtos;
using SoTags.Domain.Interfaces.DataProviders;
using SoTags.Domain.Models;

namespace SoTags.DataProvider.Providers;


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
                    Guid.NewGuid(),
                    tagDto.HasSynonyms,
                    tagDto.IsModeratorOnly,
                    tagDto.IsRequired,
                    tagDto.Count,
                    tagDto.Name
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
}
