using MediatR;

namespace SoTags.Domain.Commands;

public class RefetchTagsCommand : IRequest<bool>
{
    public int Count { get; set; } = 1000;
}
