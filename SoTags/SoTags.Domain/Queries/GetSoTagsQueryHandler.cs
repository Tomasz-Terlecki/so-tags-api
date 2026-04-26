using MediatR;
using SoTags.Domain.Dtos;
using SoTags.Domain.Interfaces.Repositories;
using SoTags.Domain.Models;

namespace SoTags.Domain.Queries;

public class GetSoTagsQueryHandler : IRequestHandler<GetSoTagsQuery, PaginatedResponseDto<SoTag>>
{
    private readonly ISoTagRepository _soTagRepository;

    public GetSoTagsQueryHandler(ISoTagRepository soTagRepository)
    {
        _soTagRepository = soTagRepository;
    }

    public async Task<PaginatedResponseDto<SoTag>> Handle(GetSoTagsQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

        var totalCount = await _soTagRepository.GetCountAsync();

        var pagedTags = await _soTagRepository.GetPagedAsync(pageNumber, pageSize, request.SortBy, request.SortDirection);

        var tags = pagedTags.Select(tag =>
            new SoTag(
                tag.Id,
                tag.HasSynonyms,
                tag.IsModeratorOnly,
                tag.IsRequired,
                tag.Count,
                tag.Name
            )
        );

        // Calculate total pages
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        return new PaginatedResponseDto<SoTag>(
            tags,
            pageNumber,
            pageSize,
            totalCount,
            totalPages
        );
    }
}
