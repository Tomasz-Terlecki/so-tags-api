using MediatR;
using Microsoft.AspNetCore.Mvc;
using SoTags.Domain.Models;
using SoTags.Domain.Queries;
using SoTags.Domain.Commands;

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

    [HttpPost("refetch")]
    public async Task<IActionResult> Refetch([FromQuery] int count = 1000)
    {
        await _mediator.Send(new RefetchTagsCommand(count));
        return Ok(new { message = $"Successfully refetched {count} tags." });
    }
}
