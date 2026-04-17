namespace BackendApi.Models;

public class Module
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
}