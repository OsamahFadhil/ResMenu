FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Menufy.sln", "./"]
COPY ["src/Menufy.Domain/Menufy.Domain.csproj", "src/Menufy.Domain/"]
COPY ["src/Menufy.Application/Menufy.Application.csproj", "src/Menufy.Application/"]
COPY ["src/Menufy.Infrastructure/Menufy.Infrastructure.csproj", "src/Menufy.Infrastructure/"]
COPY ["src/Menufy.API/Menufy.API.csproj", "src/Menufy.API/"]
COPY ["tests/Menufy.Application.Tests/Menufy.Application.Tests.csproj", "tests/Menufy.Application.Tests/"]

# Restore dependencies
RUN dotnet restore "Menufy.sln"

# Copy the rest of the code
COPY . .

# Build the application
WORKDIR "/src/src/Menufy.API"
RUN dotnet build "Menufy.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Menufy.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5000
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Menufy.API.dll"]
