using System.Collections.ObjectModel;
using DuckLog.Models;

namespace DuckLog.Models;

public class Module : IModule
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    
    public int TotalTasks => GetTasks().Count;
    public int CompletedTasks => GetTasks().Count(task => task.Status == TaskStatus.Done);
    
    public Module(int id, string name, string description) : base(id)
    {
        Name = name;
        Description = description;
    }
}
