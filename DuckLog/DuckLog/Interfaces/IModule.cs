using System.Collections.ObjectModel;
using DuckLog.Models;

namespace DuckLog.Models;

/// <summary>
/// Represents a module containing a collection of tasks.
/// </summary>
public abstract class IModule
{
    private static int _nextId = 1;

    /// <summary>
    /// Gets the module identifier.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Gets or sets the module name.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets or sets the module description.
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// Collection of tasks associated with the module.
    /// </summary>
    private ObservableCollection<ITask> Tasks { get; set; } = new ObservableCollection<ITask>();

    /// <summary>
    /// Gets the total number of tasks in the module.
    /// </summary>
    public int TotalTasks
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the number of completed tasks in the module.
    /// </summary>
    public int CompletedTasks
    {
        get;
        set;
    }

    /// <summary>
    /// Initializes a new module instance with an auto-generated identifier.
    /// </summary>
    public IModule()
    {
        Id = _nextId++;
    }

    /// <summary>
    /// Returns the tasks of the module.
    /// </summary>
    public ObservableCollection<ITask> GetTasks()
    {
        return Tasks;
    }

    /// <summary>
    /// Adds a task to the module.
    /// </summary>
    /// <param name="task">Task to add.</param>
    public void AddTask(ITask task)
    {
        Tasks.Add(task);
    }
}