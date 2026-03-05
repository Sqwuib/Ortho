using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ortho.Api.Data;
using Ortho.Api.Models;

namespace Ortho.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;

    public AuthController(AppDbContext db)
    {
        _db = db;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Username and password are required.");

        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("Email is required.");

        var normalizedEmail = request.Email.Trim().ToLower();

        var emailExists = await _db.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail);
        if (emailExists)
            return Conflict("Email is already in use.");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Username = request.Username.Trim(),
            Email = normalizedEmail,
            PasswordHash = hashedPassword,
            FirstName = request.FirstName ?? string.Empty,
            LastName = request.LastName ?? string.Empty,
            PhoneNumber = request.PhoneNumber ?? string.Empty,
            Team = request.Team ?? string.Empty
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new { message = "User registered successfully." });
    }
}

public class RegisterRequest
{
    public string? Username { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Team { get; set; }
}