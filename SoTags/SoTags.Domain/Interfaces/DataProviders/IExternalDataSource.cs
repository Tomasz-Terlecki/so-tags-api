namespace SoTags.Domain.Interfaces.DataProviders;

public interface IExternalDataSource<TExternalDataModel>
    where TExternalDataModel : class
{
    /// <summary>
    /// Gets the total count of available external data models.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Total count of available models</returns>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of external data models.
    /// </summary>
    /// <param name="count">Number of models to download</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of downloaded models</returns>
    Task<IEnumerable<TExternalDataModel>> GetAsync(int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all available external data models.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>All available models</returns>
    Task<IEnumerable<TExternalDataModel>> GetAllAsync(CancellationToken cancellationToken = default);
}
