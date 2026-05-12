using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DuckLog.Models;

[Table("app_user")]
public class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [NotMapped]
    [JsonPropertyName("Password")] 
    public string Password { get; set; } = string.Empty;

    [Column("password_hash")]
    [JsonPropertyName("PasswordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [NotMapped]
    public static User? CurrentUser { get; set; }
}