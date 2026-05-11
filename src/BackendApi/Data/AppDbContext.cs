using DuckLog.Models;
using Microsoft.EntityFrameworkCore;
using Module = BackendApi.Models.Module;


namespace BackendApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Module> Modules { get; set; }
    public DbSet<Models.Task> Tasks { get; set; }
    public DbSet<Models.User> Users { get; set; }
}