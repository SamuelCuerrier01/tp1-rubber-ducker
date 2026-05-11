using BackendApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Allow physical devices on the same network to reach the API.
builder.WebHost.UseUrls("http://0.0.0.0:5050");


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.")));


builder.Services.AddControllers();

var app = builder.Build();


app.MapControllers();

app.MapGet("/api/modules", async (AppDbContext db) =>
{
    var modules = await db.Modules.ToListAsync();
    return Results.Ok(modules);
});

app.MapPost("/api/modules", async (BackendApi.Models.Module newModule, AppDbContext db) =>
{
    db.Modules.Add(newModule);
     
    await db.SaveChangesAsync();
    
    return Results.Created($"/api/modules/{newModule.Id}", newModule);
});

app.MapPost("/api/tasks", async (BackendApi.Models.Task newTask, AppDbContext db) =>
{
    db.Tasks.Add(newTask);
    await db.SaveChangesAsync();
    return Results.Created($"/api/tasks/{newTask.Id}", newTask);
});

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

app.MapPost("/api/users", async (BackendApi.Models.User newUser, AppDbContext db) =>
{
    string rawPassword = newUser.Password?.Trim();

    if (string.IsNullOrWhiteSpace(rawPassword))
    {
        return Results.BadRequest("Password is required.");
    }

    newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(rawPassword);

    db.Users.Add(newUser);
    await db.SaveChangesAsync();

    newUser.Password = string.Empty;
    newUser.PasswordHash = string.Empty;
    
    return Results.Created($"/api/users/{newUser.Id}", newUser);
});

app.Run();