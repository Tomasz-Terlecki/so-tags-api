namespace SoTags.Domain.Dtos;

/// <summary>
/// Generic DTO for paginated responses.
/// </summary>
/// <typeparam name="T">The type of items in the response.</typeparam>
public record PaginatedResponseDto<T>(
    IEnumerable<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages
)
where T : class;
