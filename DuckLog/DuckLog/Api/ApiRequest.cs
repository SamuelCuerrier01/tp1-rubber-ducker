using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DuckLog.Models;
using Task = System.Threading.Tasks.Task;
using TaskStatus = DuckLog.Models.TaskStatus;

namespace DuckLog.Api;

public static class ApiRequest
{
    public static async Task Run(ModuleDemo module)
    {
        ObservableCollection<ITask> tasks = module.GetTasks();
        string apiKey = "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";
        StringBuilder listeCompacte = new StringBuilder();
        foreach (var task in tasks)
        {
            listeCompacte.Append(
                $"{task.Name}|{task.Description}|{task.StartDate.ToString("yyyy-MM-dd")}|{task.StartDate.ToString("yyyy-MM-dd")}, ");
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

        var response = await client.PostAsync(
            "https://api.groq.com/openai/v1/chat/completions",
            new StringContent(json, Encoding.UTF8, "application/json")
        );

        var responseString = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(responseString);
        var content = doc
            .RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();
        string[] infos = content.Split('|');
        tasks[0].Description = content;

        Console.WriteLine($"Name : {infos[0]}");
        Console.WriteLine($"Desc : {infos[1]}");
        Console.WriteLine($"Start : {infos[2]}");
        Console.WriteLine($"End : {infos[3]}");

        try
        {
            module.AddTask(new Models.Task(infos[0], infos[1], DateTime.Parse(infos[2]), DateTime.Parse(infos[3]), TaskStatus.Todo));
        }
        catch
        {
            Console.WriteLine("La tache n'a pas pu être générée");
            throw;
        }
    }
}