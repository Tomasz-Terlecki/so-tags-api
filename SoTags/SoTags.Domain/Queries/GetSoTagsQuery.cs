using MediatR;
using SoTags.Domain.Models;

namespace SoTags.Domain.Queries;

public record GetSoTagsQuery : IRequest<IEnumerable<SoTag>>
{
}
