using MediatR;
using Microsoft.AspNetCore.Mvc;
using SoTags.Domain.Models;
using SoTags.Domain.Queries;

namespace SoTags.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SoTagsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SoTagsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IEnumerable<SoTag>> Get()
    {
        var query = new GetSoTagsQuery();
        return await _mediator.Send(query);
    }
}
