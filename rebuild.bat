@echo off
echo Cleaning up Docker containers and images...
docker-compose down -v
docker system prune -f

echo.
echo Rebuilding and starting all services...
docker-compose up --build

pause
