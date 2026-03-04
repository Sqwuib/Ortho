using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ortho.Api.Data;
using Ortho.Api.Models;

namespace Ortho.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProjectsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Project>>> Get()
    {
        var projects = await _db.Projects
            .OrderBy(p => p.Id)
            .ToListAsync();

        return Ok(projects);
    }

    [HttpPost]
    public async Task<ActionResult<Project>> Create([FromBody] Project project)
    {
        project.Id = 0; // ensure EF treats it as new
        project.CreatedAt = DateTime.UtcNow;

        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = project.Id }, project);
    }
}