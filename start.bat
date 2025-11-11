@echo off
echo Starting Menufy Application...
echo.
echo This will start all services using Docker Compose.
echo Make sure Docker Desktop is running!
echo.
pause

docker-compose up --build

pause
