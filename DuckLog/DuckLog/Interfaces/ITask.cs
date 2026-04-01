using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DuckLog.Models;

public interface ITask : INotifyPropertyChanged

{
    int Id { get; set; }
    string Name { get; set; }
    string Description { get; set; }
    DateTime EndDate { get; set; }
    DateTime StartDate { get; set; }
    TaskStatus Status { get; set; }
    ObservableCollection<ITask> SubTasks { get; set; }

    public void Edit(string name, string description, DateTime endDate, DateTime startDate, TaskStatus status);
}