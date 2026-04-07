namespace DuckLog.Models;

/// <summary>
/// Représente une base de données factice contenant des modules pour les tests ou le développement.
/// </summary>
public static class FakeModuleDatabase
{
    /// <summary>
    /// Collection des modules disponibles dans la base de données fictive.
    /// </summary>
    public static List<Module> MyModules { get; set; } = new List<Module>();

    public static Module GetModuleById(int id)
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