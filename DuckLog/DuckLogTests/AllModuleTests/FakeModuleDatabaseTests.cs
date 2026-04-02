

using DuckLog.Models;

namespace DuckLogTests.AllModuleTests;

public class FakeModuleDatabaseTests : IDisposable
{
    // Constructor runs BEFORE every test
    public FakeModuleDatabaseTests()
    {
        FakeModuleDatabase.MyModules.Clear();
    }

    // Dispose runs AFTER every test
    public void Dispose()
    {
        FakeModuleDatabase.MyModules.Clear();
    }

    [Fact]
    public void Modules_Initially_IsEmpty()
    {
        Assert.Empty(FakeModuleDatabase.MyModules);
    }

    [Fact]
    public void Add_New_Module()
    {
        var testModule = new Module(1,"NameTest", "Description Test");
        
        FakeModuleDatabase.MyModules.Add(testModule);
        
        Assert.Single(FakeModuleDatabase.MyModules);
        
        var savedModule = FakeModuleDatabase.MyModules.First();
        Assert.Equal("NameTest", savedModule.Name);
        Assert.Equal("Description Test", savedModule.Description);
    }

    [Fact]
    public void Clear_Existing_Modules()
    {
        FakeModuleDatabase.MyModules.Add(new Module(1,"NameTest 1", "Description Test 1"));
        FakeModuleDatabase.MyModules.Add(new Module(2,"NameTest 2", "Description Test 2"));
        
        Assert.Equal(2, FakeModuleDatabase.MyModules.Count);
        
        FakeModuleDatabase.MyModules.Clear();
        
        Assert.Empty(FakeModuleDatabase.MyModules);
    }

    [Fact]
    public void Remove_Existing_Module()
    {
        var moduleToRemove = new Module(1,"Remove", "Description Test 1");
        var moduleToKeep = new Module(2,"Keep", "Description Test 2");
        
        FakeModuleDatabase.MyModules.Add(moduleToRemove);
        FakeModuleDatabase.MyModules.Add(moduleToKeep);
        
        bool wasRemoved = FakeModuleDatabase.MyModules.Remove(moduleToRemove);
        
        Assert.True(wasRemoved);
        Assert.Single(FakeModuleDatabase.MyModules);
        Assert.Equal("Keep", FakeModuleDatabase.MyModules.First().Name);
    }
}