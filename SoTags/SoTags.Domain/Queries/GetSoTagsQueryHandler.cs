using MediatR;
using SoTags.Domain.Interfaces.DataProviders;
using SoTags.Domain.Models;

namespace SoTags.Domain.Queries;

public class GetSoTagsQueryHandler : IRequestHandler<GetSoTagsQuery, IEnumerable<SoTag>>
{
    private readonly ISoTagProvider _soTagProvider;

    public GetSoTagsQueryHandler(ISoTagProvider soTagProvider)
    {
        _soTagProvider = soTagProvider;
    }

    public async Task<IEnumerable<SoTag>> Handle(GetSoTagsQuery request, CancellationToken cancellationToken)
    {
        return await _soTagProvider.GetAsync(100, cancellationToken);
    }
}
