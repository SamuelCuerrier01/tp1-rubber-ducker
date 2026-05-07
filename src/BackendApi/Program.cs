using BackendApi.Data;
using BackendApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Database config
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.Parse("8.0.0-mysql")));

var app = builder.Build();


//dotnet ef database update --> use it in terminal,
//we must use it every time after a pull from GitHub so database is synced

//Get all modules
app.MapGet("/api/modules", async (AppDbContext db) =>
{
    var modules = await db.Modules.ToListAsync();
    return Results.Ok(modules);
});

//Create a module
app.MapPost("/api/modules", async (Module newModule, AppDbContext db) =>
{
    db.Modules.Add(newModule);
    
    await db.SaveChangesAsync();
    
    return Results.Created($"/api/modules/{newModule.Id}", newModule);
});

//Create a task
app.MapPost("/api/tasks", async (BackendApi.Models.Task newTask, AppDbContext db) =>
{
    db.Tasks.Add(newTask);
    await db.SaveChangesAsync();
    return Results.Created($"/api/tasks/{newTask.Id}", newTask);
});

//Update task
app.MapPut("/api/tasks/{id}", async (int id, BackendApi.Models.Task updatedTask, AppDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task is null) return Results.NotFound();

    task.Name = updatedTask.Name;
    task.Description = updatedTask.Description;
    task.StartDate = updatedTask.StartDate;
    task.EndDate = updatedTask.EndDate;
    task.Status = updatedTask.Status;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

//Get a precise module
app.MapGet("/api/modules/{id}", async (int id, AppDbContext db) =>
        await db.Modules.FindAsync(id) is { } m 
        ? Results.Ok(m) 
        : Results.NotFound());

app.Run();