using System.Net.Http.Json;
using DuckLog;
using DuckLog.Models;

namespace DuckLog.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(Constants.BaseApiUrl) };
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        try
        {
            var loginData = new
            {
                Email = email.Trim(),
                Password = password.Trim()
            };
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<User>();
            }

            var errorBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Login failed: {(int)response.StatusCode} {response.ReasonPhrase} - {errorBody}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Connexion error: {ex.Message}");
        }
        return null;
    }
}