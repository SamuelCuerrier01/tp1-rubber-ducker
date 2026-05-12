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

//Get all modules
app.MapGet("/api/modules", async (int userId, AppDbContext db) =>
{
    var modules = await db.Modules
        .Where(m => m.UserId == userId)
        .Include(m => m.Tasks)
        .ToListAsync();

    foreach (var m in modules)
    {
        m.TotalTasks = await db.Tasks.CountAsync(t => t.ModuleId == m.Id);
        m.CompletedTasks = await db.Tasks.CountAsync(t => t.ModuleId == m.Id && t.Status == "Done");
    }

    return Results.Ok(modules);
});


//get all tasks
app.MapGet("/api/tasks", async (AppDbContext db) =>
{
    var tasks = await db.Tasks.ToListAsync();
    return Results.Ok(tasks);
});

//Create a module
app.MapPost("/api/modules", async (Module newModule, AppDbContext db) =>
{
    if (newModule.UserId <= 0)
        return Results.BadRequest("UserId is required.");
    
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

//get tasks of a user
app.MapGet("/api/tasks/user", async (int userId, AppDbContext db) =>
{
    var tasks = await db.Tasks
        .Where(t => db.Modules
            .Any(m => m.Id == t.ModuleId && m.UserId == userId))
        .ToListAsync();

    return Results.Ok(tasks);
});


//delete a module
app.MapDelete("/api/modules/{id}", async (int id, AppDbContext db) =>
{
    var module = await db.Modules.FindAsync(id);
    if (module is null) return Results.NotFound();

    db.Modules.Remove(module);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

//delete a task
app.MapDelete("/api/tasks/{id}", async (int id, AppDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task is null) return Results.NotFound();

    db.Tasks.Remove(task);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

// Create a user (Register)
app.MapPost("/api/users", async (BackendApi.Models.User newUser, AppDbContext db) =>
{
    string rawPassword = newUser.Password?.Trim();

    if (string.IsNullOrWhiteSpace(rawPassword))
        return Results.BadRequest("Password is required.");

    newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(rawPassword);

    db.Users.Add(newUser);
    await db.SaveChangesAsync();

    newUser.Password = string.Empty;
    newUser.PasswordHash = string.Empty;

    return Results.Created($"/api/users/{newUser.Id}", newUser);
});

// Login
app.MapPost("/api/auth/login", async (HttpContext http, AppDbContext db) =>
{
    var body = await http.Request.ReadFromJsonAsync<BackendApi.Models.User>();
    if (body == null) return Results.BadRequest("Invalid request.");

    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == body.Email.Trim());
    if (user == null) return Results.Unauthorized();

    bool valid = BCrypt.Net.BCrypt.Verify(body.Password.Trim(), user.PasswordHash);
    if (!valid) return Results.Unauthorized();

    user.PasswordHash = string.Empty;
    return Results.Ok(user);
});

app.Run();