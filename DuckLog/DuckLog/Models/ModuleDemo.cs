using System.Collections.ObjectModel;
using DuckLog.Models;

namespace DuckLog.Models;

public class ModuleDemo : IModule
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    
    public int TotalTasks => GetTasks().Count;
    public int CompletedTasks => GetTasks().Count(task => task.Status == TaskStatus.Done);
    
    public ModuleDemo(string name, string description) : base()
    {
        Name = name;
        Description = description;
    }
}
