using Microsoft.AspNetCore.Mvc;
using Ortho.Api.Models;

namespace Ortho.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Project>> Get()
    {
        var projects = new List<Project>
        {
            new() { Id = 1, Name = "Ortho - Initial Project" },
            new() { Id = 2, Name = "Demo Project" }
        };

        return Ok(projects);
    }
}