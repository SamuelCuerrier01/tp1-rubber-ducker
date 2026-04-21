namespace DuckLog.Models
{
    /// <summary>
    /// Représente l'état d'avancement d'une tâche dans le système DuckLog.
    /// </summary>
    public enum TaskStatus
    {
        /// <summary>
        /// La tâche est à faire et n'a pas encore été commencée.
        /// </summary>
        Todo,

        /// <summary>
        /// La tâche est en cours de réalisation.
        /// </summary>
        InProgress,

        /// <summary>
        /// La tâche est terminée.
        /// </summary>
        Done
  
}