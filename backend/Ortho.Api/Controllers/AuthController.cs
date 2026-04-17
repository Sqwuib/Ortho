using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ortho.Api.Data;
using Ortho.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace Ortho.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    // POST: /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("Email is required.");

        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Password is required.");

        var normalizedEmail = request.Email.Trim().ToLower();

        // Email uniqueness check
        var emailExists = await _db.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail);
        if (emailExists)
            return Conflict("Email is already in use.");

        // Hash password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Create user
        var user = new User
        {
            Email = normalizedEmail,
            PasswordHash = hashedPassword,
            FirstName = request.FirstName ?? string.Empty,
            LastName = request.LastName ?? string.Empty,
            PhoneNumber = request.PhoneNumber ?? string.Empty,
            Team = request.Team ?? string.Empty
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Return safe response (never return PasswordHash)
        return Ok(new
        {
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.Team
        });
    }

    // POST: /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Invalid email or password.");
        
        string normalizedEmail = request.Email.Trim().ToLower();

        // Find user by email
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
        
        if (user == null)
            return Unauthorized("Invalid email or password.");

        // Verify password
        bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!passwordValid)
            return Unauthorized("Invalid email or password.");
        
        var keyString = _config["Jwt:Key"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),          
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            token = tokenString
        });

    }
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var user = await _db.Users.FindAsync(userId);

        if (user == null)
            return NotFound();

        return Ok(new
        {
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Team
        });
    }
    
}

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Team { get; set; }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

}