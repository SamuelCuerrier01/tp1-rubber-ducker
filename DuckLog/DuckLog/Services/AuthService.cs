using System.Net.Http.Json;
using DuckLog.Models;

namespace DuckLog.Api;

public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("https://10.0.2.2:7001/") };
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        try
        {
            var loginData = new { Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync("api/user/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<User>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Connexion error: {ex.Message}");
        }
        return null;
    }
}