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
    public async Task<IActionResult> Refetch([FromBody] RefetchTagsCommand command)
    {
var result =         await _mediator.Send(command);

        if (result == -1)
        {
            return StatusCode(500, new { message = "An error occurred while refetching tags." });
        }

        return Ok(new { message = $"Successfully refetched {result} tags." });
    }
}
