using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DuckLog.Models;
using Task = System.Threading.Tasks.Task;
using TaskStatus = DuckLog.Models.TaskStatus;

namespace DuckLog.Api;

public static class ApiRequest
{
    private static readonly HttpClient HttpClient = new HttpClient(new SocketsHttpHandler
    {
        PooledConnectionLifetime = TimeSpan.Zero,
        ConnectTimeout = TimeSpan.FromSeconds(15)
    });
    
    public static async Task Run(Module module)
    {
        
        var tasks = module.GetTasks();
        string apiKey = "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";
        StringBuilder listeCompacte = new StringBuilder();
        foreach (var task in tasks)
        {
            listeCompacte.Append(
                $"{task.Name}|{task.Description}|{task.StartDate.ToString("yyyy-MM-dd")}|{task.EndDate.ToString("yyyy-MM-dd")}, ");
        }
        
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");


        var body = new
        {
            model = "llama-3.3-70b-versatile",
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = $"Here is a list of task in a task list : {listeCompacte}. " +
                              $"From this list, you must create a new task that correlates with those tasks, use keywords." +
                              $" you will need to return exactly in this format : <task Name>|<task Description>|<task Start Date (yyyy-MM-dd)>|<task End Date (yyyy-MM-dd)>" +
                              $"IT IS VERY IMPORTANT THAT YOU DON'T WRITE ANYTHING ELSE BESIDES : <task Name>|<task Description>|<task Start Date (yyyy-MM-dd)>|<task End Date (yyyy-MM-dd)>"
                }
            }
        };

        var json = JsonSerializer.Serialize(body);
    
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
        requestMessage.Headers.Add("Authorization", $"Bearer {apiKey}");
        requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await HttpClient.SendAsync(requestMessage);
            
            response.EnsureSuccessStatusCode(); 

            var responseString = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseString);
            var content = doc
                .RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (string.IsNullOrWhiteSpace(content))
                throw new Exception("AI returned empty content.");

            string[] infos = content.Split('|');
            
            if (infos.Length < 4)
                throw new Exception("AI did not return the expected 4 parameters.");

            string newName = infos[0].Trim();
            string newDesc = infos[1].Trim();
            DateTime newStart = DateTime.Parse(infos[2].Trim());
            DateTime newEnd = DateTime.Parse(infos[3].Trim());
            
            if (newStart < DateTime.Today)
                newStart = DateTime.Today;

            if (newEnd < DateTime.Today)
                newEnd = DateTime.Today.AddDays(2);

            if (newEnd < newStart)
                newEnd = newStart.AddDays(2);

            Console.WriteLine($"Name : {newName}");
            Console.WriteLine($"Desc : {newDesc}");
            Console.WriteLine($"Start : {newStart}");
            Console.WriteLine($"End : {newEnd}");
            
            var newTask = new
            {
                Name = newName,
                Description = newDesc,
                StartDate = newStart,
                EndDate = newEnd,
                Status = TaskStatus.Todo.ToString(),
                ModuleId = module.Id
            };

            using var backendClient = new HttpClient();
            backendClient.BaseAddress = new Uri("http://10.0.2.2:5050");
            await backendClient.PostAsJsonAsync("/api/tasks", newTask);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"La tache n'a pas pu être générée: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            throw;
        }
    }
}