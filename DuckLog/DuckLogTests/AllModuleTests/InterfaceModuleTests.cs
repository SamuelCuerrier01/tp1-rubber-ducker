using System.Collections.ObjectModel;
using System.ComponentModel;
using DuckLog.Models;

namespace DuckLogTests.AllModuleTests;

public class InterfaceModuleTests
{
    private class TestableModule : IModule
    {
        public TestableModule(int id) : base(id)
        {
        }
    }

    private class FakeTask : ITask
    {
        public int Id { get;  set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DuckLog.Models.TaskStatus Status { get; set; }
        public ObservableCollection<ITask> SubTasks { get; set; } = new ObservableCollection<ITask>();
        
        public event PropertyChangedEventHandler? PropertyChanged;
    }

    [Fact]
    public void Constructor_Test()
    {
        int expectedId = 0;
        var module = new TestableModule(expectedId);
        Assert.Equal(expectedId, module.Id);
    }
    
    [Fact]
    public void GetTasks_Test()
    {
        var module = new TestableModule(1); 
        
        var tasks = module.GetTasks();
        
        Assert.Empty(tasks);
    }
    
    [Fact]
    public void AddTask_Test()
    {
        var module = new TestableModule(1);
        var newTask = new FakeTask { Id = 100 };
        
        module.AddTask(newTask);
        
        var tasks = module.GetTasks();
        Assert.Single(tasks);
        Assert.Equal(100, tasks.First().Id);
    }
    
    [Fact]
    public void AddTask_ReplaceTaskWithSameId_Test()
    {
        var module = new TestableModule(1);
        var originalTask = new FakeTask { Id = 99 };
        var updatedTask = new FakeTask { Id = 99 };
        
        module.AddTask(originalTask);
        module.AddTask(updatedTask);
        
        var tasks = module.GetTasks();
        
        Assert.Single(tasks);
        Assert.Same(updatedTask, tasks.First());
    }
}