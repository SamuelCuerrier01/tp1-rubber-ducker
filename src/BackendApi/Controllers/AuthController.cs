using BackendApi.Data;
using BackendApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var email = (loginRequest.Email ?? string.Empty).Trim();
        var password = (loginRequest.Password ?? string.Empty).Trim();

        Console.WriteLine($"Login attempt for: {email}");
        
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        
        if (user == null || user.PasswordHash.Trim() != password)
            return Unauthorized(new { message = "Email ou mot de passe incorrect" });

        return Ok(user); 
    }
}