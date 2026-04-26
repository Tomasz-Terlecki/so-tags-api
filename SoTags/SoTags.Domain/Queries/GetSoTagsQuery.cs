using MediatR;
using SoTags.Domain.Dtos;
using SoTags.Domain.Enums;
using SoTags.Domain.Models;

namespace SoTags.Domain.Queries;

public record GetSoTagsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    SortBy SortBy = SortBy.None,
    SortDirection SortDirection = SortDirection.Ascending
) : IRequest<PaginatedResponseDto<SoTag>>;

