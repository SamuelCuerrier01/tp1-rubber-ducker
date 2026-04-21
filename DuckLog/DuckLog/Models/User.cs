// il faut utiliser : dotnet add package BCrypt.Net-Next
// dotnet add package MySqlConnector

namespace DuckLog.Models;

public class User : IUser
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; } = string.Empty;
    public static User? CurrentUser { get; private set; }
    public string PasswordHash { get; set; } = string.Empty;

    public User(string Name, string Email, string Password)
    {
        //Get id from BD
        this.Name = Name;
        this.Email = Email;
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password);
    }

    public void ConnectUser(string inputEmail, string inputPassword)
    {
        User? userFromDb = GetUserInDB(inputEmail);

        if (userFromDb == null) return;
        
        bool isValid = BCrypt.Net.BCrypt.Verify(inputPassword, userFromDb.PasswordHash);

        if (isValid)
        {
            CurrentUser = userFromDb;
            Console.WriteLine($"Bienvenue {CurrentUser.Name}");
        }
    }

    private User? GetUserInDB(string email)
    {
        /*
        en cours de code
        using var connection = new MySqlConnection("Server=myServer;Database=BDuckDB;Uid=myUser;Pwd=myPassword;");
        
        string sql = "SELECT id, name, email, password_hash AS PasswordHash FROM app_user WHERE email = @Email";
    
        return connection.QueryFirstOrDefault<User>(sql, new { Email = email });*/
        return null;
    }
}