using DuckLog.Models;

namespace DuckLog.Data;

public static class Data
{
    private static Dictionary<int, IModule> _modules = new Dictionary<int, IModule>();
    
    public static Dictionary<int, IModule> GetModules => _modules;

    public static void AddModule(IModule module)
    {
        if (!_modules.ContainsKey(module.Id))
        {
            _modules.TryAdd(module.Id, module);
        }
    }

    public static void RemoveModule(IModule module)
    {
        _modules.Remove(module.Id);
    }

    public static void UpdateModule(IModule module)
    {
        _modules[module.Id] = module;
    }
}