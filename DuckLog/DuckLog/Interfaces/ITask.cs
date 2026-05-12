using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DuckLog.Models;

/// <summary>
/// Represents a task model.
/// </summary>
public interface ITask : INotifyPropertyChanged

{
    /// <summary>
    /// Task identifier.
    /// </summary>
    int Id { get; set; }
    /// <summary>
    /// Task name.
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// Task description.
    /// </summary>
    string Description { get; set; }
    /// <summary>
    /// Task end date.
    /// </summary>
    DateTime EndDate { get; set; }
    /// <summary>
    /// Task start date.
    /// </summary>
    DateTime StartDate { get; set; }
    /// <summary>
    /// Task status.
    /// </summary>
    TaskStatus Status { get; set; }
    /// <summary>
    /// Subtasks collection.
    /// </summary>
    ObservableCollection<ITask> SubTasks { get; set; }
    
    public void Edit(string name, string description, DateTime endDate, DateTime startDate, TaskStatus status);
}