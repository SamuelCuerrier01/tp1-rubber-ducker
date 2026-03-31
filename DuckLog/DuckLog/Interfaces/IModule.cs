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