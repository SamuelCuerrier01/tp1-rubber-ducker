namespace DuckLog.Models;

public interface IUser
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public static User? CurrentUser { get; private set; }
    public string PasswordHash { get; set; }

    public void ConnectUser(string inputEmail, string inputPassword);
}