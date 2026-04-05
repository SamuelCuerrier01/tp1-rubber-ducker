using System.Collections.ObjectModel;

namespace DuckLog.Models
{
    /// <summary>
    /// Représente une base de données factice contenant des modules pour les tests ou le développement.
    /// </summary>
    public static class FakeModuleDatabase
    {
        /// <summary>
        /// Collection observable des modules disponibles dans la base de données factice.
        /// Cette collection peut être utilisée pour la liaison de données dans l'interface utilisateur.
        /// </summary>
        public static ObservableCollection<Module> MyModules { get; set; } = new ObservableCollection<Module>();
    }
}