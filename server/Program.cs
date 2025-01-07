using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// הוספת CORS לאפליקציה - מאפשר לכל מקור לפנות ל-API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // מאפשר לכל מקור לפנות ל-API
              .AllowAnyMethod()  // מאפשר כל שיטה HTTP (GET, POST, PUT, DELETE וכו')
              .AllowAnyHeader(); // מאפשר כל כותרת (header)
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
app.UseCors("AllowAll");  // מגדיר את מדיניות ה-CORS שתשפיע על כל הנתיבים

// הפעלת Swagger UI
if (app.Environment.IsDevelopment())  // הצגת Swagger רק בסביבות פיתוח
{
    app.UseSwagger();
    app.UseSwaggerUI();  // מציג את הממשק של Swagger
}

app.MapGet("/", () => "Hello World!");

// הניתוב לשליפת כל המשימות
app.MapGet("/todos", async (ToDoDbContext dbContext) =>
{
    return await dbContext.Items.ToListAsync();
});

// הניתוב להוספת משימה חדשה
app.MapPost("/todos", async (ToDoDbContext dbContext, Item newItem) =>
{
    dbContext.Items.Add(newItem);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/todos/{newItem.Id}", newItem);
});

// הניתוב לעדכון משימה
app.MapPut("/todos/{id}", async (ToDoDbContext dbContext, int id, Item updatedItem) =>
{
    var item = await dbContext.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;

    await dbContext.SaveChangesAsync();
    return Results.NoContent();
});

// הניתוב למחיקת משימה
app.MapDelete("/todos/{id}", async (ToDoDbContext dbContext, int id) =>
{
    var item = await dbContext.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    dbContext.Items.Remove(item);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();