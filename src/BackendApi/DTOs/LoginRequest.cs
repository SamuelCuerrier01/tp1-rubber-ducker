using System.Text.Json.Serialization;

namespace BackendApi.DTOs;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    [System.Text.Json.Serialization.JsonPropertyName("Password")]
    public string Password { get; set; } = string.Empty;
}