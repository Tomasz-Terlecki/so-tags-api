using MediatR;
using SoTags.Domain.Interfaces.DataProviders;
using SoTags.Domain.Interfaces.Repositories;

namespace SoTags.Domain.Commands;

public class RefetchTagsCommandHandler : IRequestHandler<RefetchTagsCommand, bool>
{
    private readonly ISoTagProvider _soTagProvider;
    private readonly ISoTagRepository _soTagRepository;

    public RefetchTagsCommandHandler(ISoTagProvider soTagProvider, ISoTagRepository soTagRepository)
    {
        _soTagProvider = soTagProvider ?? throw new ArgumentNullException(nameof(soTagProvider));
        _soTagRepository = soTagRepository ?? throw new ArgumentNullException(nameof(soTagRepository));
    }

    public async Task<bool> Handle(RefetchTagsCommand request, CancellationToken cancellationToken)
    {
        await _soTagRepository.RemoveAllAsync();
        await _soTagRepository.SaveChangesAsync();

        var tags = await _soTagProvider.GetAsync(request.Count, cancellationToken);

        foreach (var tag in tags)
        {
            await _soTagRepository.AddAsync(tag);
        }

        var saveResult = await _soTagRepository.SaveChangesAsync();
        if (saveResult != tags.Count())
        {
            return false;
        }

        return true;
    }
}
