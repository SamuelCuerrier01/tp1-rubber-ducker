using System.Collections.ObjectModel;

namespace DuckLog.Models;

public static class FakeModuleDatabase
{
    public static ObservableCollection<Module> MyModules { get; set; } = new ObservableCollection<Module>();
}