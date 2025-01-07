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

// שימוש בהגדרת CORS
app.UseCors("AllowAll");

// הפעלת Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.MapGet("/", () => "Hello World!");

app.MapGet("/todos", async (ToDoDbContext dbContext) =>
{
    return await dbContext.Items.ToListAsync();
});

app.MapPost("/todos", async (ToDoDbContext dbContext, Item newItem) =>
{
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
