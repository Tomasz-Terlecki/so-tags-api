using MediatR;
using SoTags.Domain.Models;

namespace SoTags.Domain.Queries;

public class GetSoTagsQueryHandler : IRequestHandler<GetSoTagsQuery, IEnumerable<SoTag>>
{
    public GetSoTagsQueryHandler()
    {
    }

    public Task<IEnumerable<SoTag>> Handle(GetSoTagsQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<SoTag>>(new List<SoTag>
        {
            new(1, "C#", "A programming language developed by Microsoft."),
            new(2, "ASP.NET", "A web framework for building web applications."),
            new(3, "Entity Framework", "An object-relational mapper for .NET.")
        });
    }
}
