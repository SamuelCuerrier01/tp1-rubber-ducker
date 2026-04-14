// il faut utiliser : dotnet add package BCrypt.Net-Next
// dotnet add package MySqlConnector
// Packages requis:
// dotnet add package BCrypt.Net-Next
// dotnet add package MySqlConnector
// dotnet add package Dapper

using Dapper;
using MySqlConnector;
using BCryptTool = BCrypt.Net.BCrypt;

namespace DuckLog.Models;

public class User : IUser
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public static User? CurrentUser { get; private set; }
    public string PasswordHash { get; set; } = string.Empty;

    public void ConnectUser(string inputEmail, string inputPassword)
    {
        User? userFromDb = GetUserInDB(inputEmail);

        if (userFromDb == null)
            return;

        bool isValid = BCryptTool.Verify(inputPassword, userFromDb.PasswordHash);

        if (isValid)
        {
            CurrentUser = userFromDb;
        }
    }

    private User? GetUserInDB(string email)
    {
        string connectionString = "Server=localhost;Database=BDuckDB;Uid=root;Pwd=;";
        try
        {
            using var connection = new MySqlConnection(connectionString);
            string sql =
                "SELECT id AS Id, name AS Name, email AS Email, password_hash AS PasswordHash FROM app_user WHERE email = @Email";
            return connection.QueryFirstOrDefault<User>(sql, new { Email = email });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur BD : {ex.Message}");
            return null;
        }
    }
}

