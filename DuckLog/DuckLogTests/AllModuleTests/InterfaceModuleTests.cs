using System.Collections.ObjectModel;
using System.ComponentModel;
using DuckLog.Models;
using TaskStatus = DuckLog.Models.TaskStatus;

namespace DuckLogTests.AllModuleTests;

public class InterfaceModuleTests
{
    private class TestableModule : IModule
    {
        public TestableModule() : base() { }
    }

    private class FakeTask : ITask
    {
        public void Edit(string name, string description, DateTime endDate, DateTime startDate, TaskStatus status)
            => throw new NotImplementedException();

        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TaskStatus Status { get; set; }
        public ObservableCollection<ITask> SubTasks { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    [Fact]
    public void Constructor_Id_AutoIncrement_Test()
    {
        var module1 = new TestableModule();
        var module2 = new TestableModule();

        Assert.Equal(module1.Id + 1, module2.Id);
    }

    [Fact]
    public void GetTasks_Empty_Test()
    {
        var module = new TestableModule();

        Assert.Empty(module.GetTasks());
    }

    [Fact]
    public void TotalTasks_Zero_Test()
    {
        var module = new TestableModule();

        Assert.Equal(0, module.TotalTasks);
    }

    [Fact]
    public void AddTask_Test()
    {
        var module  = new TestableModule();
        var newTask = new FakeTask { Id = 100 };

        module.AddTask(newTask);

        var tasks = module.GetTasks();
        Assert.Single(tasks);
        Assert.Equal(100, tasks.First().Id);
    }

    [Fact]
    public void AddTask_TotalTasks_Increments_Test()
    {
        var module = new TestableModule();

        module.AddTask(new FakeTask { Id = 1 });
        module.AddTask(new FakeTask { Id = 2 });

        Assert.Equal(2, module.TotalTasks);
    }

    [Fact]
    public void CompletedTasks_Zero_Test()
    {
        var module = new TestableModule();

        module.AddTask(new FakeTask { Id = 1, Status = TaskStatus.Todo });
        module.AddTask(new FakeTask { Id = 2, Status = TaskStatus.InProgress });

        Assert.Equal(0, module.CompletedTasks);
    }

    [Fact]
    public void CompletedTasks_Count_Test()
    {
        var module = new TestableModule();

        module.AddTask(new FakeTask { Id = 1, Status = TaskStatus.Done });
        module.AddTask(new FakeTask { Id = 2, Status = TaskStatus.Done });
        module.AddTask(new FakeTask { Id = 3, Status = TaskStatus.Todo });

        Assert.Equal(2, module.CompletedTasks);
    }
    
    [Fact]
    public void CompletedTasks_Zero_When_NoTasks_Test()
    {
        var module = new TestableModule();

        Assert.Equal(0, module.CompletedTasks);
    }
}