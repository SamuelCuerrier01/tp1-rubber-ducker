using System.Collections.ObjectModel;

namespace DuckLog.Models;

/// <summary>
/// Represents a module containing a collection of tasks.
/// </summary>
public abstract class IModule
{
    /// <summary>
    /// Gets the module identifier.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Collection of tasks associated with the module.
    /// </summary>
    private ObservableCollection<ITask> Tasks { get; set; } = new ObservableCollection<ITask>();

    /// <summary>
    /// Initializes a new module instance.
    /// </summary>
    /// <param name="id">Module identifier.</param>
    public IModule(int id)
    {
        Id = id;
    }

    /// <summary>
    /// Returns the tasks of the module.
    /// </summary>
    /// <returns>Collection of tasks.</returns>
    public ObservableCollection<ITask> GetTasks()
    {
        return Tasks;
    }

    /// <summary>
    /// Adds a task or replaces it if it already exists.
    /// </summary>
    /// <param name="task">Task to add.</param>
    public void AddTask(ITask task)
    {
        if (!(Tasks.Any(x => x.Id == task.Id)))
        {
            Tasks.Add(task);
        }
        else
        {
            Tasks.Remove(Tasks.First(x => x.Id == task.Id));
            Tasks.Add(task);
        }
    }
}