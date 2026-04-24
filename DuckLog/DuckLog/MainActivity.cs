using Android.App;

namespace DuckLog
{
    /// <summary>
    /// Activité principale de l'application DuckLog.
    /// Sert de point d'entrée initial et charge l'interface utilisateur principale.
    /// </summary>
    [Activity]
    public class MainActivity : Activity
    {
        /// <summary>
        /// Méthode appelée lors de la création de l'activité.
        /// Initialise l'interface utilisateur et les composants nécessaires.
        /// </summary>
        /// <param name="savedInstanceState">État précédent de l'activité, si disponible.</param>
        protected override void OnCreate(Android.OS.Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
    }
}