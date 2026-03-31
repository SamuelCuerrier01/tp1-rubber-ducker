using System.Collections.ObjectModel;
using DuckLog.Models;
using TaskStatus = System.Threading.Tasks.TaskStatus;
namespace DuckLog.Models;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class Task : ObservableObject, ITask
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _name;
        
    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private DateTime _endDate;

    [ObservableProperty]
    private DateTime _startDate;

    [ObservableProperty]
    private TaskStatus _status;

    [ObservableProperty] private ObservableCollection<ITask> _subTasks = new ObservableCollection<ITask>();

    public Task(string name, string description, DateTime endDate, DateTime startDate, TaskStatus status)
    {
        _name = name;
        _description = description;
        _endDate = endDate;
        _startDate = startDate;
        _status = status;
    }
}