# Local Development Guide

## Recommended: Use Docker Compose

The easiest way to develop is using Docker Compose, which handles all setup automatically:

```bash
docker-compose up --build
```

This gives you:
- ✅ PostgreSQL database (no installation needed)
- ✅ Automatic migrations
- ✅ Backend API
- ✅ Frontend app
- ✅ pgAdmin for database management

## Alternative: Local Development Without Docker

If you prefer to run services locally:

### Prerequisites

1. **Install PostgreSQL 16**
   - Download: https://www.postgresql.org/download/windows/
   - During installation:
     - Port: 5432
     - Password: postgres (or update appsettings.Development.json)
     - Remember to start PostgreSQL service

2. **Install .NET 8 SDK**
   - Download: https://dotnet.microsoft.com/download/dotnet/8.0

3. **Install Node.js 20+**
   - Download: https://nodejs.org/

### Backend Setup

1. **Verify PostgreSQL is running**:
   ```bash
   # Check if PostgreSQL service is running
   # Windows: Services -> PostgreSQL
   # Or test connection:
   psql -U postgres -h localhost
   ```

2. **Navigate to backend**:
   ```bash
   cd C:\Users\pc1\Documents\menufy\backend\src
   ```

3. **Create Initial Migration** (first time only):
   ```bash
   dotnet ef migrations add InitialCreate --project Menufy.Infrastructure --startup-project Menufy.API
   ```

4. **Apply Migration**:
   ```bash
   dotnet ef database update --project Menufy.Infrastructure --startup-project Menufy.API
   ```

5. **Run the API**:
   ```bash
   cd Menufy.API
   dotnet run
   ```

   API will be available at: http://localhost:5000

### Frontend Setup

1. **Navigate to frontend**:
   ```bash
   cd C:\Users\pc1\Documents\menufy\frontend
   ```

2. **Install dependencies**:
   ```bash
   npm install
   ```

3. **Create .env file**:
   ```bash
   copy .env.sample .env
   ```

4. **Run development server**:
   ```bash
   npm run dev
   ```

   Frontend will be available at: http://localhost:3000

## Database Management

### Using pgAdmin (with Docker)

If running Docker Compose, access pgAdmin at http://localhost:5050

### Using psql (local PostgreSQL)

```bash
psql -U postgres -d menufy
```

Common commands:
```sql
-- List all tables
\dt

-- View users
SELECT * FROM "Users";

-- View restaurants
SELECT * FROM "Restaurants";

-- Drop database (reset)
DROP DATABASE menufy;
CREATE DATABASE menufy;
```

## Environment Configuration

### Backend (appsettings.Development.json)

Already configured for localhost:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=menufy;Username=postgres;Password=postgres"
  }
}
```

### Frontend (.env)

```env
NUXT_PUBLIC_API_BASE=http://localhost:5000/api
```

## Development Workflow

### Making Database Changes

1. **Modify entity** in `Menufy.Domain/Entities`
2. **Create migration**:
   ```bash
   cd backend/src
   dotnet ef migrations add YourMigrationName --project Menufy.Infrastructure --startup-project Menufy.API
   ```
3. **Apply migration**:
   ```bash
   dotnet ef database update --project Menufy.Infrastructure --startup-project Menufy.API
   ```

### Adding New Features

1. **Create entity** (Domain layer)
2. **Create DTOs** (Application layer)
3. **Create command/query** (Application layer)
4. **Create handler** (Application layer)
5. **Add validator** (Application layer)
6. **Create controller** (API layer)
7. **Create migration** (as above)

### Hot Reload

- **Backend**: dotnet watch (automatic with `dotnet run`)
- **Frontend**: Vite dev server (automatic with `npm run dev`)

## Troubleshooting Local Development

### "No such host is known" Error

This means you're trying to connect to `db` (Docker hostname) but PostgreSQL is not running or not accessible.

**Solution**:
- Install PostgreSQL locally, OR
- Use Docker Compose instead

### "Database does not exist" Error

**Solution**:
```bash
# Connect to PostgreSQL
psql -U postgres

# Create database
CREATE DATABASE menufy;

# Exit
\q

# Run migrations
cd backend/src
dotnet ef database update --project Menufy.Infrastructure --startup-project Menufy.API
```

### Port Already in Use

**Backend (5000)**:
```bash
# Find process
netstat -ano | findstr :5000

# Kill process (use PID from above)
taskkill /PID <PID> /F
```

**Frontend (3000)**:
```bash
# Find process
netstat -ano | findstr :3000

# Kill process
taskkill /PID <PID> /F
```

### Entity Framework Tools Not Found

**Solution**:
```bash
dotnet tool install --global dotnet-ef
```

## VS Code / Visual Studio Setup

### VS Code Extensions (Recommended)

- C# Dev Kit
- Volar (Vue)
- Tailwind CSS IntelliSense
- PostgreSQL (for database management)
- REST Client (for testing APIs)

### Launch Configurations

Create `.vscode/launch.json`:

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (API)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/backend/src/Menufy.API/bin/Debug/net8.0/Menufy.API.dll",
      "args": [],
      "cwd": "${workspaceFolder}/backend/src/Menufy.API",
      "stopAtEntry": false,
      "serverReadyAction": {
        "action": "openExternally",
        "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
      },
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  ]
}
```

## Performance Tips

- Use EF Core change tracking wisely
- Add database indexes for frequently queried fields
- Use `.AsNoTracking()` for read-only queries
- Cache QR codes instead of regenerating
- Optimize images before uploading

## Best Practices

1. **Always create migrations** when changing entities
2. **Use DTOs** instead of returning entities directly
3. **Validate input** using FluentValidation
4. **Handle errors** properly with try-catch
5. **Use async/await** for all database operations
6. **Test APIs** using Swagger before frontend integration

## Quick Commands Reference

```bash
# Backend
dotnet restore                          # Restore packages
dotnet build                            # Build solution
dotnet run                              # Run API
dotnet watch                            # Run with hot reload
dotnet ef migrations add <Name>         # Create migration
dotnet ef database update               # Apply migrations
dotnet ef database drop                 # Drop database

# Frontend
npm install                             # Install dependencies
npm run dev                             # Development server
npm run build                           # Production build
npm run preview                         # Preview production build

# Docker
docker-compose up                       # Start all services
docker-compose up --build               # Rebuild and start
docker-compose down                     # Stop all services
docker-compose down -v                  # Stop and remove volumes
docker-compose logs -f api              # Follow API logs
```

## Recommended: Hybrid Approach

For best development experience:

1. **Use Docker for database**: `docker-compose up db pgadmin`
2. **Run backend locally**: `cd backend/src/Menufy.API && dotnet run`
3. **Run frontend locally**: `cd frontend && npm run dev`

This gives you:
- ✅ No PostgreSQL installation needed
- ✅ Fast backend debugging
- ✅ Fast frontend hot reload
- ✅ Database management via pgAdmin

To do this, update docker-compose.yml to only run specific services:

```bash
docker-compose up db pgadmin
```

Then in separate terminals:
```bash
# Terminal 1 - Backend
cd backend/src/Menufy.API
dotnet run

# Terminal 2 - Frontend
cd frontend
npm run dev
```

---

**Recommendation**: Start with full Docker Compose to ensure everything works, then switch to hybrid approach for development if needed.
