using MediatR;
using SoTags.Domain.Models;

namespace SoTags.Domain.Queries;

public class GetSoTagsQuery : IRequest<IEnumerable<SoTag>>
{
}
