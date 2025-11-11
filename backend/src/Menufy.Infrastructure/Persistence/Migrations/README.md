# Migrations

This folder will contain Entity Framework Core migrations.

## Creating Migrations

When you modify entities, create a migration:

```bash
cd backend/src
dotnet ef migrations add YourMigrationName --project Menufy.Infrastructure --startup-project Menufy.API
```

## Applying Migrations

```bash
dotnet ef database update --project Menufy.Infrastructure --startup-project Menufy.API
```

## Note

For Docker deployment, the application uses `EnsureCreated()` which automatically creates the database schema without requiring migration files.
