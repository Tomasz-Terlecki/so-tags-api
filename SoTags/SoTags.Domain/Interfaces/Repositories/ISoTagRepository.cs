using SoTags.Domain.Enums;
using SoTags.Domain.Models;

namespace SoTags.Domain.Interfaces.Repositories;

public interface ISoTagRepository
{
    /// <summary>
    /// Gets a paged collection of SoTags with optional sorting.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="sortBy">The field to sort by.</param>
    /// <param name="sortDirection">The sort direction.</param>
    /// <returns>A collection of SoTags for the specified page.</returns>
    Task<IEnumerable<SoTag>> GetPagedAsync(int pageNumber, int pageSize, SortBy sortBy = SortBy.None, SortDirection sortDirection = SortDirection.Ascending);

    /// <summary>
    /// Gets the total count of SoTags.
    /// </summary>
    /// <returns>The total count.</returns>
    Task<int> GetCountAsync();

    /// <summary>
    /// Gets a SoTag by its ID.
    /// </summary>
    /// <param name="id">The SoTag ID.</param>
    /// <returns>The SoTag if found; otherwise null.</returns>
    Task<SoTag?> GetByIdAsync(Guid id);

    /// <summary>
    /// Adds a SoTag to the repository.
    /// </summary>
    /// <param name="soTag">The SoTag to add.</param>
    Task AddAsync(SoTag soTag);

    /// <summary>
    /// Removes a SoTag by its ID.
    /// </summary>
    /// <param name="id">The SoTag ID to remove.</param>
    Task RemoveAsync(Guid id);

    /// <summary>
    /// Removes all SoTags from the repository.
    /// </summary>
    Task RemoveAllAsync();

    /// <summary>
    /// Saves all changes to the data store.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync();
}
