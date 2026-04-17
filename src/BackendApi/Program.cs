using BackendApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Tell the endpoint to ask AppDbContext for the data
app.MapGet("/api/modules", async (AppDbContext db) =>
{
    // This translates to: SELECT * FROM Modules;
    var modules = await db.Modules.ToListAsync();
    return Results.Ok(modules);
});

app.Run();