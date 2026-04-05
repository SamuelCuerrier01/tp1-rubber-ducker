using System.Collections.ObjectModel;
using DuckLog.Models;

namespace DuckLog.Models
{
    /// <summary>
    /// Représente un module spécifique contenant des tâches.
    /// Hérite de <see cref="IModule"/>.
    /// </summary>
    public class Module : IModule
    {
        /// <summary>
        /// Nom du module.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Description du module.
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// Nombre total de tâches dans le module.
        /// </summary>
        public int TotalTasks => GetTasks().Count;

        /// <summary>
        /// Nombre de tâches terminées dans le module.
        /// </summary>
        public int CompletedTasks => GetTasks().Count(task => task.Status == TaskStatus.Done);

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Module"/>.
        /// </summary>
        /// <param name="id">Identifiant unique du module.</param>
        /// <param name="name">Nom du module.</param>
        /// <param name="description">Description du module.</param>
        public Module(int id, string name, string description) : base(id)
        {
            Name = name;
            Description = description;
        }
    }
}