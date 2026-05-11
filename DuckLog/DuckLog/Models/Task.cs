using System.Collections.ObjectModel;
using DuckLog.Models;
using TaskStatus = System.Threading.Tasks.TaskStatus;
using System.Text.Json.Serialization;
namespace DuckLog.Models;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class Task : ObservableObject, ITask
{
    private static int _next_id = 1;
    [ObservableProperty]
    private int _id;
    
    [ObservableProperty]
    private int _moduleId;

    [ObservableProperty]
    private string _name;
        
    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private DateTime _endDate;

    [ObservableProperty]
    private DateTime _startDate;

    [ObservableProperty]
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    private TaskStatus _status;

    [ObservableProperty]
    [property: JsonIgnore]
    private ObservableCollection<ITask> _subTasks = new ObservableCollection<ITask>();

    public Task() { }

    // 2. Custom constructor (without the static ID generator)
    public Task(string name, string description, DateTime startDate, DateTime endDate, TaskStatus status, int moduleId)
    {
        Name = name;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
        ModuleId = moduleId;
    }

    public void Edit(string name, string description, DateTime startDate, DateTime endDate, TaskStatus status)
    {
        Name = name;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
    }
}