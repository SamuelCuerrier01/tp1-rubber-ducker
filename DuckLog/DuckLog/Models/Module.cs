using DuckLog.Models;

namespace DuckLog.Models;

/// <summary>
/// Concrete implementation of a module.
/// </summary>
public class Module : IModule
{
    /// <summary>
    /// Initializes a new module with a name and description.
    /// </summary>
    /// <param name="name">The module name.</param>
    /// <param name="description">The module description.</param>
    public Module(string name, string description)
    {
        Name = name;
        Description = description;
    }
}