using System.Collections.ObjectModel;

namespace DuckLog.Models;

public abstract class IModule
{
    public int Id { get; }
    private ObservableCollection<ITask> Tasks { get; set; } = new ObservableCollection<ITask>();

    public IModule()
    {
        Id = Data.Data.GetIdModule;
    }

    public ObservableCollection<ITask> GetTasks()
    {
        return Tasks;
    }
    public void AddTask(ITask task)
    {
        Tasks.Add(task);
    }
}