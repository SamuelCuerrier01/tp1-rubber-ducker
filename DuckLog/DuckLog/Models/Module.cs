using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using DuckLog.Models;

namespace DuckLog.Models;

/// <summary>
/// Concrete implementation of a module.
/// </summary>
public class Module : IModule
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    [JsonPropertyName("tasks")]
    public ObservableCollection<Task> Tasks { get; set; } = new ObservableCollection<Task>();

    public Module() { }

    public Module(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public ObservableCollection<ITask> GetTasks() 
    {
        return new ObservableCollection<ITask>(Tasks);
    }
}