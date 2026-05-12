using DuckLog.Models;

namespace DuckLog.Data
{
    /// <summary>
    /// Classe statique servant de stockage en mémoire pour les modules et les tâches.
    /// Fournit des méthodes pour ajouter, supprimer et mettre à jour les modules.
    /// </summary>
    public static class Data
    {
        /// <summary>
        /// Dictionnaire interne stockant les modules par leur identifiant.
        /// </summary>
        private static Dictionary<int, IModule> _modules = new Dictionary<int, IModule>();

        /// <summary>
        /// Accès en lecture au dictionnaire des modules.
        /// </summary>
        public static Dictionary<int, IModule> GetModules => _modules;

        /// <summary>
        /// Compteur interne pour générer des identifiants uniques de tâches.
        /// </summary>
        private static int _idTask = 1;

        /// <summary>
        /// Compteur interne pour générer des identifiants uniques de modules.
        /// </summary>
        private static int _idModule = 1;

        /// <summary>
        /// Ajoute un module à la base de données si son identifiant n'existe pas déjà.
        /// </summary>
        /// <param name="module">Le module à ajouter.</param>
        public static void AddModule(IModule module)
        {
            if (!_modules.ContainsKey(module.Id))
            {
                _modules.TryAdd(module.Id, module);
            }
        }

        /// <summary>
        /// Supprime un module de la base de données.
        /// </summary>
        /// <param name="module">Le module à supprimer.</param>
        public static void RemoveModule(IModule module)
        {
            _modules.Remove(module.Id);
        }

        /// <summary>
        /// Met à jour un module existant dans la base de données.
        /// </summary>
        /// <param name="module">Le module avec les nouvelles valeurs.</param>
        public static void UpdateModule(IModule module)
        {
            _modules[module.Id] = module;
        }

        /// <summary>
        /// Retourne un identifiant unique pour une nouvelle tâche et incrémente le compteur.
        /// </summary>
        public static int GetIdTask => _idTask++;

        /// <summary>
        /// Retourne un identifiant unique pour un nouveau module et incrémente le compteur.
        /// </summary>
        public static int GetIdModule => _idModule++;
    }
}