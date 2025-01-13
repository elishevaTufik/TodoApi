using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// הוספת CORS לאפליקציה - מאפשר לכל מקור לפנות ל-API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() 
              .AllowAnyMethod() 
              .AllowAnyHeader();
    });
});

// הוספת קונפיגורציה למסד נתונים (MySQL)
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"), 
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToDoDB"))));

// הוספת Swagger
builder.Services.AddEndpointsApiExplorer();  // עבור Swagger
builder.Services.AddSwaggerGen();  // עבור Swagger

var app = builder.Build();


// אחרי הוספת DbContext לשירותים
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
        if (!db.Database.CanConnect())
        {
            throw new Exception("Unable to connect to the database.");
        }
    }
    Console.WriteLine("Database connection successful.");
}
catch (Exception ex)
{
    Console.WriteLine($"Database connection failed: {ex.Message}");
    return; // מונע מהאפליקציה להמשיך לפעול במקרה של שגיאה
}


// שימוש בהגדרת CORS
app.UseCors("AllowAll");

// הפעלת Swagger UI
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.MapGet("/", () => "todo api");

app.MapGet("/todos", async (ToDoDbContext dbContext) =>
{
    return await dbContext.Items.ToListAsync();
});

app.MapPost("/todos", async (ToDoDbContext dbContext, Item newItem) =>
{
    if (string.IsNullOrWhiteSpace(newItem.Name))
    return Results.BadRequest("Task name cannot be empty.");

    dbContext.Items.Add(newItem);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/todos/{newItem.Id}", newItem);
});

app.MapPut("/todos/{id}", async (ToDoDbContext dbContext, int id, Item updatedItem) =>
{
    var item = await dbContext.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    item.IsComplete = updatedItem.IsComplete;

    await dbContext.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todos/{id}", async (ToDoDbContext dbContext, int id) =>
{
    var item = await dbContext.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    dbContext.Items.Remove(item);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
