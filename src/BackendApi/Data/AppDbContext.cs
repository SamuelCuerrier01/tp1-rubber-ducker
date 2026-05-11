using BackendApi.Models;
using Microsoft.EntityFrameworkCore;


namespace BackendApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    //represents table in database ex: Modules, Tasks, etc. 
    public DbSet<Module> Modules { get; set; }
    public DbSet<Models.Task> Tasks { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Models.Task>()
            .HasOne(t => t.Module)
            .WithMany(m => m.Tasks)
            .HasForeignKey(t => t.ModuleId)
            .OnDelete(DeleteBehavior.Cascade); 
    }
}