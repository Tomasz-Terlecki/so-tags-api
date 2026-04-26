using MediatR;

namespace SoTags.Domain.Commands;

public record RefetchTagsCommand(int Count = 1000) : IRequest<bool>;
