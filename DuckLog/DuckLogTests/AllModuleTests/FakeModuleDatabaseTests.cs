using DuckLog.Models;
using TaskStatus = DuckLog.Models.TaskStatus;

namespace DuckLogTests.AllModuleTests;

[Collection("FakeModuleDatabase")]
public class FakeModuleDatabaseTests : IDisposable
{
    public FakeModuleDatabaseTests()
    {
        FakeModuleDatabase.MyModules.Clear();
    }

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
        var testModule = new Module("NameTest", "Description Test");

        FakeModuleDatabase.MyModules.Add(testModule);

        Assert.Single(FakeModuleDatabase.MyModules);
        Assert.Equal("NameTest",         FakeModuleDatabase.MyModules.First().Name);
        Assert.Equal("Description Test", FakeModuleDatabase.MyModules.First().Description);
    }

    [Fact]
    public void Clear_Existing_Modules()
    {
        FakeModuleDatabase.MyModules.Add(new Module("NameTest 1", "Description Test 1"));
        FakeModuleDatabase.MyModules.Add(new Module("NameTest 2", "Description Test 2"));

        Assert.Equal(2, FakeModuleDatabase.MyModules.Count);

        FakeModuleDatabase.MyModules.Clear();

        Assert.Empty(FakeModuleDatabase.MyModules);
    }

    [Fact]
    public void Remove_Existing_Module()
    {
        var moduleToRemove = new Module("Remove", "Description Test 1");
        var moduleToKeep   = new Module("Keep",   "Description Test 2");

        FakeModuleDatabase.MyModules.Add(moduleToRemove);
        FakeModuleDatabase.MyModules.Add(moduleToKeep);

        bool wasRemoved = FakeModuleDatabase.MyModules.Remove(moduleToRemove);

        Assert.True(wasRemoved);
        Assert.Single(FakeModuleDatabase.MyModules);
        Assert.Equal("Keep", FakeModuleDatabase.MyModules.First().Name);
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
        var task   = new DuckLog.Models.Task("task name", "task desc", default, default, TaskStatus.Todo);
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
        var task1   = new DuckLog.Models.Task("task 1", "desc 1", default, default, TaskStatus.Todo);
        var task2   = new DuckLog.Models.Task("task 2", "desc 2", default, default, TaskStatus.Todo);

        module1.AddTask(task1);
        module2.AddTask(task2);
        FakeModuleDatabase.MyModules.Add(module1);
        FakeModuleDatabase.MyModules.Add(module2);

        var result = FakeModuleDatabase.GetTaskById(task2.Id);

        Assert.NotNull(result);
        Assert.Equal(task2.Id, result.Id);
    }
}