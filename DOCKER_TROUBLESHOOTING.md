# Docker Troubleshooting Guide

## Common Issues and Solutions

### Issue 1: Frontend npm ci fails (SOLVED)

**Error**: `npm ci can only install with an existing package-lock.json`

**Solution**: We've updated the Dockerfile to use `npm install` instead of `npm ci`.

### Issue 2: Port Already in Use

**Error**: `Port 3000/5000/5432 is already in use`

**Solution**:
```bash
# Stop all containers
docker-compose down

# Check what's using the ports
netstat -ano | findstr :3000
netstat -ano | findstr :5000
netstat -ano | findstr :5432

# Kill the processes or change ports in docker-compose.yml
```

### Issue 3: Database Connection Fails

**Error**: `Could not connect to database`

**Solution**:
1. Wait 10-15 seconds for PostgreSQL to fully start
2. Check database health: `docker-compose logs db`
3. Verify database is running: `docker ps`

### Issue 4: Build Cache Issues

**Error**: Various build errors

**Solution**:
```bash
# Clean everything and rebuild
docker-compose down -v
docker system prune -f
docker-compose up --build
```

Or use the provided script:
```bash
rebuild.bat
```

### Issue 5: CORS Errors in Frontend

**Error**: CORS policy blocking requests

**Solution**:
- Check API is running: `docker ps`
- Verify API URL in frontend .env
- Check CORS policy in backend Program.cs

### Issue 6: Database Migrations Not Running

**Error**: Tables don't exist

**Solution**:
The API automatically runs migrations on startup (see Program.cs).
Wait for the API to fully start, or check logs:
```bash
docker-compose logs api
```

## Clean Start Procedure

If you're having persistent issues:

1. **Stop everything**:
   ```bash
   docker-compose down -v
   ```

2. **Clean Docker**:
   ```bash
   docker system prune -a --volumes
   ```

3. **Rebuild**:
   ```bash
   docker-compose up --build
   ```

## Checking Logs

### View all logs:
```bash
docker-compose logs
```

### View specific service logs:
```bash
docker-compose logs api
docker-compose logs frontend
docker-compose logs db
```

### Follow logs in real-time:
```bash
docker-compose logs -f api
```

## Manual Database Access

### Using Docker:
```bash
docker exec -it menufy-db psql -U postgres -d menufy
```

### Using pgAdmin:
1. Go to http://localhost:5050
2. Login: admin@menufy.com / admin
3. Add server with host: `db`

## Service Health Checks

### Check if containers are running:
```bash
docker ps
```

### Check specific service:
```bash
docker-compose ps api
docker-compose ps frontend
docker-compose ps db
```

### Restart specific service:
```bash
docker-compose restart api
docker-compose restart frontend
```

## Build Without Docker

If Docker continues to have issues, you can run locally:

### Backend:
```bash
cd backend/src/Menufy.API
dotnet restore
dotnet run
```

### Frontend:
```bash
cd frontend
npm install
npm run dev
```

### Database:
Install PostgreSQL locally and update connection strings.

## Getting Help

1. Check logs for specific error messages
2. Verify all prerequisites are installed
3. Ensure Docker Desktop is running and up to date
4. Check Windows firewall settings
5. Try running as Administrator if permission issues

## Success Indicators

When everything is working, you should see:

1. **4 containers running**:
   - menufy-db
   - menufy-api
   - menufy-frontend
   - menufy-pgadmin

2. **Accessible endpoints**:
   - Frontend: http://localhost:3000 (shows landing page)
   - API: http://localhost:5000 (returns 404 on root, which is normal)
   - Swagger: http://localhost:5000/swagger (shows API documentation)
   - pgAdmin: http://localhost:5050 (shows login page)

3. **API logs show**:
   - "Now listening on: http://[::]:5000"
   - "Application started"

4. **Frontend logs show**:
   - "Listening on http://0.0.0.0:3000"

## Performance Tips

- Allocate more memory to Docker Desktop (Settings > Resources)
- Ensure WSL 2 is being used (for Windows)
- Close unnecessary applications
- Use SSD for Docker data

## Still Having Issues?

1. Check Docker Desktop dashboard for error messages
2. Review the main README.md
3. Try the GETTING_STARTED.md guide
4. Ensure all files were created correctly
5. Verify no antivirus is blocking Docker
