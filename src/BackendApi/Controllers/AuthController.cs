using Dapper;
using DuckLog.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // The Backend uses the connection string from appsettings.json
    private readonly string _connectionString = "Server=localhost;Database=BDuckDB;Uid=root;Pwd=;";

    [HttpPost("login")]
    public IActionResult Login([FromBody] User loginRequest)
    {
        using var connection = new MySqlConnection(_connectionString);
        string sql = "SELECT id, name, email, password_hash as PasswordHash FROM app_user WHERE email = @Email";
        var userFromDb = connection.QueryFirstOrDefault<User>(sql, new { Email = loginRequest.Email });

        if (userFromDb == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, userFromDb.PasswordHash))
        {
            return Unauthorized("Invalid email or password.");
        }

        return Ok(userFromDb);
    }
}