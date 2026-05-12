namespace DuckLog.Models;
public static class FakeModuleDatabase
{public static List<ModuleDemo> MyModules { get; set; } = new List<ModuleDemo>();

    public static ModuleDemo GetModuleById(int id)
    {
        foreach (var module in MyModules)
        {
            if (id == module.Id)
            {
                return module;
            }
        }
        return null;
    }
    
    public static ITask GetTaskById(int id)
    {
        foreach (var module in MyModules)
        {
            foreach (var task in module.GetTasks())
            {
                if (task.Id == id)
                {
                    return task;
                }
            }
        }
        return null;
    }
}