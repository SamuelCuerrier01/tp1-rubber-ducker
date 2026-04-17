using BackendApi.Models;
using Microsoft.EntityFrameworkCore;


namespace BackendApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // This line tells EF Core to create a table called "Modules"
    public DbSet<Module> Modules { get; set; }
}