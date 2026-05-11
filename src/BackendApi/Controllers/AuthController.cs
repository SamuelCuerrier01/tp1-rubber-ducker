using BackendApi.Data;
using BackendApi.DTOs;
using Microsoft.AspNetCore.Identity;
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
        var email = loginRequest.Email.Trim();
        var password = loginRequest.Password.Trim();

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null) {
            return Unauthorized(new { message = "Email ou mot de passe incorrect" });
        }

        bool isMatch = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

        if (!isMatch)
            return Unauthorized(new { message = "Email ou mot de passe incorrect" });

        return Ok(user); 
    }
}