namespace BackendApi.Models;

public class Task
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public required string Status { get; set; }
    public int ModuleId { get; set; }
    
    public Module? Module { get; set; }
}