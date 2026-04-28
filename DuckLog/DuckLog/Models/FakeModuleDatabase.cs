namespace DuckLog.Models;

/// <summary>
/// Représente une base de données factice contenant des modules pour les tests ou le développement.
/// </summary>
public static class FakeModuleDatabase
{
    /// <summary>
    /// Collection des modules disponibles dans la base de données fictive.
    /// </summary>
    public static List<Module> MyModules { get; set; } = new List<Module>()
    {
        new Module("Sprint 1", "Premier sprint")
        {
            // La notification va se trigger au lancement
        }
    };

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
    static FakeModuleDatabase()
    {
        var module = MyModules[0];
    
        // test tache en retard
        module.AddTask(new Task(
            "Fix  bug",
            "Corriger bug",
            DateTime.Now.AddDays(-2),
            DateTime.Now.AddDays(-5),
            TaskStatus.InProgress
        ));

        // tache due bientot
        module.AddTask(new Task(
            "Code review",
            "Réviser le code du sprint",
            DateTime.Now.AddHours(10),
            DateTime.Now,
            TaskStatus.Todo
        ));
    }
}