# Task Management Fullstack Application

## Project Overview
This project is a fullstack task management application consisting of:
- **Server**: Backend API using Minimal API architecture.
- **Client**: Frontend application for interacting with the tasks.
- **Database**: A MySQL database for storing tasks.

The project is designed to provide a lightweight API solution, leveraging modern development tools and methodologies.

## Features
- **Minimal API**: Write small and focused APIs without boilerplate code.
- **Dotnet CLI**: Use .NET CLI for cross-platform development and command-line operations.
- **Database First**: Generate models and context using an existing database schema.
- **CORS Configuration**: Enable cross-origin requests for the client to interact with the API.
- **Swagger Integration**: Use Swagger for testing and documentation of the API.

---

## Backend Setup

### Database Configuration
1. **Create Table**: Use the following schema for the `Items` table:
    ```sql
    CREATE TABLE Items (
        Id INT NOT NULL AUTO_INCREMENT,
        Name VARCHAR(100),
        IsComplete TINYINT(1),
        PRIMARY KEY (Id)
    );
    ```

2. **Connection String**: Add the following connection string to `appsettings.json`:
    ```json
    "ToDoDB": "server=localhost;user=root;password=1234;database=ToDoDB"
    ```

### Entity Framework Setup
1. **Install Packages**:
    ```bash
    dotnet add package Microsoft.EntityFrameworkCore
    dotnet add package Microsoft.EntityFrameworkCore.Design
    dotnet add package Pomelo.EntityFrameworkCore.MySql
    dotnet add package Microsoft.EntityFrameworkCore.Tools
    ```

2. **Generate Models and Context**:
    ```bash
    dotnet ef dbcontext scaffold Name=ToDoDB Pomelo.EntityFrameworkCore.MySql -f -c ToDoDbContext
    ```
    - `Name`: Specifies the connection string property name in `appsettings.json`.
    - `-f`: Overwrites existing files if they exist.
    - `-c`: Specifies the name of the `DbContext` class.

3. **Generated Classes**:
    - `Item`: Represents the `Items` table.
    - `ToDoDbContext`: Manages the database context.

### Routes Mapping
Define API routes in `Program.cs`:
- **GET**: Fetch all tasks.
- **POST**: Add a new task.
- **PUT**: Update a task.
- **DELETE**: Delete a task.

Inject the `ToDoDbContext` into services and use it directly for database operations.

### CORS Configuration
Enable CORS in `Program.cs` to allow client access to the API:
```csharp
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

app.UseCors();
```

### Swagger Setup
Install and configure Swagger for API documentation and testing:
```bash
dotnet add package Swashbuckle.AspNetCore
```
Add the following to `Program.cs`:
```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

app.UseSwagger();
app.UseSwaggerUI();
```

---

## Frontend Setup

### API Service
Define API calls in `service.js`:
1. Set the base URL using Axios Config Defaults.
2. Implement API functions:
    - `getTasks`: Fetch all tasks (already implemented).
    - `addTask`: Add a new task.
    - `updateTask`: Update an existing task.
    - `deleteTask`: Delete a task.

3. Add an Axios interceptor to log errors:
    ```javascript
    axios.interceptors.response.use(
        response => response,
        error => {
            console.error(error);
            return Promise.reject(error);
        }
    );
    ```

### Running the Client
Update the API route in `service.js` with the correct port. Ensure the API is running before testing.

---

## Running the Project

### Backend
1. Open the terminal and run:
    ```bash
    dotnet run
    ```
2. Debug using VS Code:
    - Press `F5` or select `Run and Debug`.
    - Add breakpoints as needed.

### Frontend
1. Run the frontend application with the appropriate tool (e.g., `npm start`).

---

## Additional Notes
- Use [Dotnet CLI Documentation](https://learn.microsoft.com/en-us/dotnet/core/tools/) for further guidance.
- Refer to [Axios Documentation](https://axios-http.com/docs/intro) for advanced Axios features.

---
