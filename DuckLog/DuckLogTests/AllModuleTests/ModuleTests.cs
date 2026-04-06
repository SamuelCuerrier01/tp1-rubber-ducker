using DuckLog.Models;

namespace DuckLogTests.AllModuleTests;

public class ModuleTests
{
    [Fact]
    public void Constuctor_Name_Descroption_Test()
    {
        var name = "test name";
        var description = "test description";
        
        var module = new Module (name, description);
        
        Assert.Equal("test name", module.Name);
        Assert.Equal("test description", module.Description);
    }

    [Fact]
    public void TotalTasks_Zero_Test()
    {
        var name = "test name";
        var description = "test description";
        
        var module = new Module(name, description);
        
        Assert.Equal(0, module.TotalTasks);
    }
}