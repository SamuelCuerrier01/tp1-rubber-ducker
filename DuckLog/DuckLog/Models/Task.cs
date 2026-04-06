using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DuckLog.Models
{
    /// <summary>
    /// Représente une tâche pouvant contenir des sous-tâches.
    /// Implémente l'interface <see cref="ITask"/>.
    /// </summary>
    public partial class Task : ObservableObject, ITask
    {
        /// <summary>
        /// Generer le prochain identifiant pour tâche.
        /// </summary>
        [ObservableProperty]
        private static int _next_id = 1;
        
        /// <summary>
        /// Identifiant unique de la tâche.
        /// </summary>
        [ObservableProperty]
        private int _id;

        /// <summary>
        /// Nom de la tâche.
        /// </summary>
        [ObservableProperty]
        private string _name;

        /// <summary>
        /// Description détaillée de la tâche.
        /// </summary>
        [ObservableProperty]
        private string _description;

        /// <summary>
        /// Date de début de la tâche.
        /// </summary>
        [ObservableProperty]
        private DateTime _startDate;

        /// <summary>
        /// Date de fin prévue de la tâche.
        /// </summary>
        [ObservableProperty]
        private DateTime _endDate;

        /// <summary>
        /// Statut actuel de la tâche.
        /// </summary>
        [ObservableProperty]
        private TaskStatus _status;

        /// <summary>
        /// Collection des sous-tâches associées à cette tâche.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<ITask> _subTasks = new ObservableCollection<ITask>();

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Task"/>.
        /// </summary>
        /// <param name="name">Nom de la tâche.</param>
        /// <param name="description">Description de la tâche.</param>
        /// <param name="endDate">Date de fin prévue.</param>
        /// <param name="startDate">Date de début.</param>
        /// <param name="status">Statut initial de la tâche.</param>
        public Task(string name, string description, DateTime endDate, DateTime startDate, TaskStatus status)
        {
            _id = _next_id++;
            _name = name;
            _description = description;
            _endDate = endDate;
            _startDate = startDate;
            _status = status;
        }
        
        public void Edit(string name, string description, DateTime endDate, DateTime startDate, TaskStatus status)
        {
            _name = name;
            _description = description;
            _endDate = endDate;
            _startDate = startDate;
            _status = status;
        }
    }
}