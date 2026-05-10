using BackendApi.Data;
using BackendApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Database config
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.Parse("8.0.0-mysql")));

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();


//dotnet ef database update --> use it in terminal,
//we must use it every time after a pull from GitHub so database is synced

//Get all modules
app.MapGet("/api/modules", async (AppDbContext db) =>
{
    var modules = await db.Modules.ToListAsync();
    foreach (var m in modules)
    {
        m.TotalTasks = await db.Tasks.CountAsync(t => t.ModuleId == m.Id);
        m.CompletedTasks = await db.Tasks.CountAsync(t => t.ModuleId == m.Id && t.Status == "Done");
    }
    return Results.Ok(modules);
});

//Create a module
app.MapPost("/api/modules", async (Module newModule, AppDbContext db) =>
{
    db.Modules.Add(newModule);
    
    await db.SaveChangesAsync();
    
    return Results.Ok();
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
    task.ModuleId = updatedTask.ModuleId;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

//Get a precise module
app.MapGet("/api/modules/{id}", async (int id, AppDbContext db) =>
{
    var module = await db.Modules
        .Include(m => m.Tasks)
        .FirstOrDefaultAsync(m => m.Id == id);

    return module is not null ? Results.Ok(module) : Results.NotFound();
});

//get a precise task
app.MapGet("/api/tasks/{id}", async (int id, AppDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    
    if (task is null) 
    {
        return Results.NotFound();
    }
    
    return Results.Ok(task);
});

//Get the tasks of a module
app.MapGet("/api/modules/{id}/tasks", async (int id, AppDbContext db) =>
{
    var tasks = await db.Tasks.Where(t => t.ModuleId == id).ToListAsync();
    return Results.Ok(tasks);
});

app.Run();