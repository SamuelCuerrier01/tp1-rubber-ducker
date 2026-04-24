namespace BackendApi.Models;

public class Module
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
}