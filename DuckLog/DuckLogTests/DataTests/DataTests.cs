using DuckLog.Data;
using DuckLog.Models;
using TaskStatus = DuckLog.Models.TaskStatus;

namespace DuckLogTests.DataTests;

public class DataTests : IDisposable
{
    private class TestableModule : IModule
    {
    }

    public DataTests()
    {
        Data.GetModules.Clear();
    }

    public void Dispose()
    {
        Data.GetModules.Clear();
    }
    
    [Fact]
    public void AddModule_AddsModule_Test()
    {
        var module = new TestableModule();

        Data.AddModule(module);

        Assert.Single(Data.GetModules);
        Assert.True(Data.GetModules.ContainsKey(module.Id));
    }

    [Fact]
    public void AddModule_DoesNotOverwrite_WhenSameId_Test()
    {
        var module1 = new TestableModule();
        var module2 = new TestableModule();
        var duplicate = Data.GetModules;

        Data.AddModule(module1);
        Data.AddModule(module1); 

        Assert.Single(Data.GetModules);
    }

    [Fact]
    public void RemoveModule_RemovesModule_Test()
    {
        var module = new TestableModule();
        Data.AddModule(module);

        Data.RemoveModule(module);

        Assert.Empty(Data.GetModules);
    }

    [Fact]
    public void RemoveModule_OnlyRemovesTarget_Test()
    {
        var module1 = new TestableModule();
        var module2 = new TestableModule();
        Data.AddModule(module1);
        Data.AddModule(module2);

        Data.RemoveModule(module1);

        Assert.Single(Data.GetModules);
        Assert.True(Data.GetModules.ContainsKey(module2.Id));
    }
    
    [Fact]
    public void UpdateModule_ReplacesModule_Test()
    {
        var original = new TestableModule();
        Data.AddModule(original);

        var updated = new TestableModule();
        Data.GetModules[original.Id] = updated;
        Data.UpdateModule(updated);

        Assert.Equal(updated, Data.GetModules[updated.Id]);
    }

    [Fact]
    public void GetModules_Initially_Empty_Test()
    {
        Assert.Empty(Data.GetModules);
    }

    [Fact]
    public void GetModules_ReturnsAll_Test()
    {
        var module1 = new TestableModule();
        var module2 = new TestableModule();
        Data.AddModule(module1);
        Data.AddModule(module2);

        Assert.Equal(2, Data.GetModules.Count);
    }
    
    [Fact]
    public void GetIdTask_Increments_Test()
    {
        var id1 = Data.GetIdTask;
        var id2 = Data.GetIdTask;

        Assert.Equal(id1 + 1, id2);
    }
    
    [Fact]
    public void GetIdModule_Increments_Test()
    {
        var id1 = Data.GetIdModule;
        var id2 = Data.GetIdModule;

        Assert.Equal(id1 + 1, id2);
    }
    
    [Fact]
    public void GetModuleById_Returns_Correct_Module()
    {
        var module = new Module("NameTest", "Description Test");
        FakeModuleDatabase.MyModules.Add(module);

        var result = FakeModuleDatabase.GetModuleById(module.Id);

        Assert.NotNull(result);
        Assert.Equal(module.Id, result.Id);
    }

    [Fact]
    public void GetModuleById_Returns_Null_When_Not_Found()
    {
        var result = FakeModuleDatabase.GetModuleById(-1);

        Assert.Null(result);
    }

    [Fact]
    public void GetTaskById_Returns_Correct_Task()
    {
        var module = new Module("NameTest", "Description Test");
        var task = new DuckLog.Models.Task("task name", "task desc", default, default, TaskStatus.Todo);
        module.AddTask(task);
        FakeModuleDatabase.MyModules.Add(module);

        var result = FakeModuleDatabase.GetTaskById(task.Id);

        Assert.NotNull(result);
        Assert.Equal(task.Id, result.Id);
    }

    [Fact]
    public void GetTaskById_Returns_Null_When_Not_Found()
    {
        var result = FakeModuleDatabase.GetTaskById(-1);

        Assert.Null(result);
    }

    [Fact]
    public void GetTaskById_SearchesAcrossMultipleModules()
    {
        var module1 = new Module("Module 1", "Description 1");
        var module2 = new Module("Module 2", "Description 2");
        var task1 = new DuckLog.Models.Task("task 1", "desc 1", default, default, TaskStatus.Todo);
        var task2 = new DuckLog.Models.Task("task 2", "desc 2", default, default, TaskStatus.Todo);

        module1.AddTask(task1);
        module2.AddTask(task2);
        FakeModuleDatabase.MyModules.Add(module1);
        FakeModuleDatabase.MyModules.Add(module2);

        var result = FakeModuleDatabase.GetTaskById(task2.Id);

        Assert.NotNull(result);
        Assert.Equal(task2.Id, result.Id);
    }
}