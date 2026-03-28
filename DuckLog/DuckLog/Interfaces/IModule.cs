using System.Collections.ObjectModel;
using Android.Accessibilityservice.AccessibilityService;

namespace DuckLog.Models;

public interface IModule
{
    int Id { get; }
    ObservableCollection<ITask> Tasks { get; set; }

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