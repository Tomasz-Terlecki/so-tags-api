using Microsoft.AspNetCore.Mvc;
using SoTags.Domain.Models;

namespace SoTags.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SoTagsController : ControllerBase
{
    [HttpGet]
    public IEnumerable<SoTag> Get()
    {
        return [
            new(1, "Tag1", "Description1"),
            new(2, "Tag2", "Description2")
        ];
    }
}
