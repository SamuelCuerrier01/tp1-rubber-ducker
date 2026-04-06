using DuckLogTask = DuckLog.Models.Task;
using DuckLogTaskStatus = DuckLog.Models.TaskStatus;

namespace DuckLogTests.TaskTests;

public class TaskTests
{
    private DuckLogTask CreateDefaultTask(
        string name = "test name",
        string description = "test description",
        DateTime endDate = default,
        DateTime startDate = default,
        DuckLogTaskStatus status = DuckLogTaskStatus.Todo)
    {
        return new DuckLogTask(name, description, endDate, startDate, status);
    }

    [Fact]
    public void Constructor_Name_Description_Test()
    {
        var task = CreateDefaultTask();

        Assert.Equal("test name",        task.Name);
        Assert.Equal("test description", task.Description);
    }

    [Fact]
    public void Constructor_Dates_Test()
    {
        var start = new DateTime(2025, 1, 1);
        var end   = new DateTime(2025, 12, 31);

        var task = CreateDefaultTask(startDate: start, endDate: end);

        Assert.Equal(start, task.StartDate);
        Assert.Equal(end,   task.EndDate);
    }

    [Fact]
    public void Constructor_Status_Test()
    {
        var task = CreateDefaultTask(status: DuckLogTaskStatus.InProgress);

        Assert.Equal(DuckLogTaskStatus.InProgress, task.Status);
    }

    [Fact]
    public void Constructor_SubTasks_Empty_Test()
    {
        var task = CreateDefaultTask();

        Assert.NotNull(task.SubTasks);
        Assert.Empty(task.SubTasks);
    }

    [Fact]
    public void Constructor_Id_AutoIncrement_Test()
    {
        var task1 = CreateDefaultTask();
        var task2 = CreateDefaultTask();

        Assert.Equal(task1.Id + 1, task2.Id);
    }

    [Fact]
    public void Edit_Name_Description_Test()
    {
        var task = CreateDefaultTask();

        task.Edit("new name", "new description", default, default, DuckLogTaskStatus.Todo);

        Assert.Equal("new name",        task.Name);
        Assert.Equal("new description", task.Description);
    }

    [Fact]
    public void Edit_Dates_Test()
    {
        var task     = CreateDefaultTask();
        var newStart = new DateTime(2026, 3, 1);
        var newEnd   = new DateTime(2026, 6, 1);

        task.Edit(task.Name, task.Description, newEnd, newStart, task.Status);

        Assert.Equal(newStart, task.StartDate);
        Assert.Equal(newEnd,   task.EndDate);
    }

    [Fact]
    public void Edit_Status_Test()
    {
        var task = CreateDefaultTask(status: DuckLogTaskStatus.Todo);

        task.Edit(task.Name, task.Description, task.EndDate, task.StartDate, DuckLogTaskStatus.Done);

        Assert.Equal(DuckLogTaskStatus.Done, task.Status);
    }

    [Fact]
    public void Edit_Does_Not_Change_Id_Test()
    {
        var task = CreateDefaultTask();
        var originalId = task.Id;

        task.Edit("other name", "other desc", default, default, DuckLogTaskStatus.Todo);

        Assert.Equal(originalId, task.Id);
    }
}