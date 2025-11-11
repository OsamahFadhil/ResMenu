# Deployment Guide - Menufy Pro+

This guide covers deploying Menufy Pro+ to production environments.

## Table of Contents
- [Prerequisites](#prerequisites)
- [Environment Setup](#environment-setup)
- [Docker Deployment](#docker-deployment)
- [Cloud Deployment](#cloud-deployment)
- [Database Setup](#database-setup)
- [SSL/HTTPS Configuration](#sslhttps-configuration)
- [Monitoring](#monitoring)
- [Backup Strategy](#backup-strategy)

## Prerequisites

### Server Requirements
- **CPU**: 2+ cores
- **RAM**: 4GB minimum (8GB recommended)
- **Storage**: 20GB minimum
- **OS**: Linux (Ubuntu 22.04 LTS recommended)

### Software Requirements
- Docker 24.0+
- Docker Compose 2.0+
- Nginx (for reverse proxy)
- Certbot (for SSL)

## Environment Setup

### 1. Clone Repository

```bash
git clone <repository-url>
cd menufy
```

### 2. Create Production Environment Files

**Backend - `backend/src/Menufy.API/appsettings.Production.json`**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=db;Port=5432;Database=menufy_prod;Username=menufy_user;Password=STRONG_PASSWORD_HERE"
  },
  "JwtSettings": {
    "Secret": "GENERATE_STRONG_SECRET_KEY_AT_LEAST_32_CHARACTERS",
    "Issuer": "MenufyAPI",
    "Audience": "MenufyClient",
    "ExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  },
  "AppSettings": {
    "BaseUrl": "https://yourdomain.com",
    "MaxUploadSizeMB": 10
  },
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "UseSsl": true,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "FromEmail": "noreply@yourdomain.com",
    "FromName": "Menufy"
  }
}
```

**Frontend - `frontend/.env.production`**

```env
NUXT_PUBLIC_API_BASE=https://api.yourdomain.com/api
NODE_ENV=production
```

### 3. Create Production Docker Compose

**docker-compose.prod.yml**

```yaml
services:
  db:
    image: postgres:16-alpine
    container_name: menufy-db-prod
    environment:
      POSTGRES_USER: menufy_user
      POSTGRES_PASSWORD: ${DB_PASSWORD}
      POSTGRES_DB: menufy_prod
    volumes:
      - postgres_data:/var/lib/postgresql/data
    networks:
      - menufy-network
    restart: always
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U menufy_user"]
      interval: 10s
      timeout: 5s
      retries: 5

  redis:
    image: redis:7-alpine
    container_name: menufy-redis-prod
    command: redis-server --requirepass ${REDIS_PASSWORD}
    volumes:
      - redis_data:/data
    networks:
      - menufy-network
    restart: always

  api:
    build:
      context: ./backend
      dockerfile: Dockerfile.prod
    container_name: menufy-api-prod
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:5000
      - ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=menufy_prod;Username=menufy_user;Password=${DB_PASSWORD}
      - ConnectionStrings__Redis=redis:6379,password=${REDIS_PASSWORD}
    volumes:
      - ./backend/uploads:/app/uploads
    networks:
      - menufy-network
    depends_on:
      db:
        condition: service_healthy
    restart: always

  frontend:
    build:
      context: ./frontend
      dockerfile: Dockerfile.prod
    container_name: menufy-frontend-prod
    environment:
      - NUXT_PUBLIC_API_BASE=https://api.yourdomain.com/api
      - NODE_ENV=production
    networks:
      - menufy-network
    depends_on:
      - api
    restart: always

volumes:
  postgres_data:
  redis_data:

networks:
  menufy-network:
    driver: bridge
```

## Docker Deployment

### 1. Create Production Dockerfiles

**Backend - `backend/Dockerfile.prod`**

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Menufy.API/Menufy.API.csproj", "src/Menufy.API/"]
COPY ["src/Menufy.Application/Menufy.Application.csproj", "src/Menufy.Application/"]
COPY ["src/Menufy.Infrastructure/Menufy.Infrastructure.csproj", "src/Menufy.Infrastructure/"]
COPY ["src/Menufy.Domain/Menufy.Domain.csproj", "src/Menufy.Domain/"]
RUN dotnet restore "src/Menufy.API/Menufy.API.csproj"
COPY . .
WORKDIR "/src/src/Menufy.API"
RUN dotnet build "Menufy.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Menufy.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /app/uploads
ENTRYPOINT ["dotnet", "Menufy.API.dll"]
```

**Frontend - `frontend/Dockerfile.prod`**

```dockerfile
FROM node:18-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM node:18-alpine
WORKDIR /app
COPY --from=build /app/.output ./
ENV HOST=0.0.0.0
ENV PORT=3000
EXPOSE 3000
CMD ["node", "server/index.mjs"]
```

### 2. Build and Deploy

```bash
# Set environment variables
export DB_PASSWORD="your_secure_db_password"
export REDIS_PASSWORD="your_secure_redis_password"

# Build and start
docker-compose -f docker-compose.prod.yml up -d --build

# Check logs
docker-compose -f docker-compose.prod.yml logs -f

# Run migrations
docker exec -it menufy-api-prod dotnet ef database update
```

## Cloud Deployment

### AWS Deployment

#### Using EC2

1. **Launch EC2 Instance**
   - Ubuntu 22.04 LTS
   - t3.medium or larger
   - Security Group: Open ports 80, 443, 22

2. **Connect and Setup**
   ```bash
   ssh ubuntu@your-ec2-ip

   # Install Docker
   curl -fsSL https://get.docker.com -o get-docker.sh
   sudo sh get-docker.sh
   sudo usermod -aG docker ubuntu

   # Install Docker Compose
   sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
   sudo chmod +x /usr/local/bin/docker-compose
   ```

3. **Deploy Application**
   ```bash
   git clone <repository-url>
   cd menufy
   # Follow Docker Deployment steps
   ```

#### Using ECS (Elastic Container Service)

1. Create ECR repositories
2. Push Docker images
3. Create ECS cluster
4. Deploy task definitions
5. Configure Application Load Balancer

### DigitalOcean Deployment

1. **Create Droplet**
   - Ubuntu 22.04
   - 4GB RAM minimum
   - Enable monitoring

2. **Deploy Application**
   ```bash
   # Follow EC2 setup steps
   ```

### Azure Deployment

Use Azure Container Instances or Azure Kubernetes Service (AKS)

## Database Setup

### PostgreSQL Configuration

1. **Create Database User**
   ```sql
   CREATE USER menufy_user WITH PASSWORD 'strong_password';
   CREATE DATABASE menufy_prod OWNER menufy_user;
   GRANT ALL PRIVILEGES ON DATABASE menufy_prod TO menufy_user;
   ```

2. **Run Migrations**
   ```bash
   cd backend/src/Menufy.Infrastructure
   dotnet ef database update --startup-project ../Menufy.API --configuration Release
   ```

3. **Create Indexes** (if not in migrations)
   ```sql
   CREATE INDEX idx_restaurants_owner ON "Restaurants"("OwnerId");
   CREATE INDEX idx_menuitems_category ON "MenuItems"("CategoryId");
   CREATE INDEX idx_menuitems_restaurant ON "MenuItems"("RestaurantId");
   CREATE INDEX idx_refreshtokens_user ON "RefreshTokens"("UserId");
   CREATE INDEX idx_refreshtokens_token ON "RefreshTokens"("Token");
   ```

## SSL/HTTPS Configuration

### Using Nginx + Let's Encrypt

1. **Install Nginx**
   ```bash
   sudo apt update
   sudo apt install nginx
   ```

2. **Configure Nginx**

   **/etc/nginx/sites-available/menufy**
   ```nginx
   # API
   server {
       listen 80;
       server_name api.yourdomain.com;

       location / {
           proxy_pass http://localhost:5000;
           proxy_http_version 1.1;
           proxy_set_header Upgrade $http_upgrade;
           proxy_set_header Connection 'upgrade';
           proxy_set_header Host $host;
           proxy_cache_bypass $http_upgrade;
           proxy_set_header X-Real-IP $remote_addr;
           proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
           proxy_set_header X-Forwarded-Proto $scheme;
       }
   }

   # Frontend
   server {
       listen 80;
       server_name yourdomain.com www.yourdomain.com;

       location / {
           proxy_pass http://localhost:3000;
           proxy_http_version 1.1;
           proxy_set_header Upgrade $http_upgrade;
           proxy_set_header Connection 'upgrade';
           proxy_set_header Host $host;
           proxy_cache_bypass $http_upgrade;
       }
   }
   ```

3. **Enable Site**
   ```bash
   sudo ln -s /etc/nginx/sites-available/menufy /etc/nginx/sites-enabled/
   sudo nginx -t
   sudo systemctl reload nginx
   ```

4. **Install Certbot**
   ```bash
   sudo apt install certbot python3-certbot-nginx
   sudo certbot --nginx -d yourdomain.com -d www.yourdomain.com -d api.yourdomain.com
   ```

5. **Auto-Renewal**
   ```bash
   sudo certbot renew --dry-run
   ```

## Monitoring

### 1. Application Logs

```bash
# View API logs
docker logs menufy-api-prod -f

# View Frontend logs
docker logs menufy-frontend-prod -f

# View Database logs
docker logs menufy-db-prod -f
```

### 2. Health Checks

Add endpoints:
- API: `GET /api/health`
- Database: `GET /api/health/db`

### 3. Metrics (Optional)

Install Prometheus + Grafana:

```yaml
# Add to docker-compose.prod.yml
prometheus:
  image: prom/prometheus
  volumes:
    - ./prometheus.yml:/etc/prometheus/prometheus.yml
  ports:
    - "9090:9090"

grafana:
  image: grafana/grafana
  ports:
    - "3001:3000"
  volumes:
    - grafana_data:/var/lib/grafana
```

## Backup Strategy

### Database Backups

1. **Automated Backups**

   Create `/opt/menufy/backup.sh`:
   ```bash
   #!/bin/bash
   TIMESTAMP=$(date +%Y%m%d_%H%M%S)
   BACKUP_DIR="/opt/menufy/backups"

   # Create backup
   docker exec menufy-db-prod pg_dump -U menufy_user menufy_prod > $BACKUP_DIR/menufy_$TIMESTAMP.sql

   # Compress
   gzip $BACKUP_DIR/menufy_$TIMESTAMP.sql

   # Delete backups older than 30 days
   find $BACKUP_DIR -name "menufy_*.sql.gz" -mtime +30 -delete
   ```

2. **Schedule with Cron**
   ```bash
   crontab -e
   # Add: Daily backup at 2 AM
   0 2 * * * /opt/menufy/backup.sh
   ```

### File Backups

Backup uploads directory:
```bash
rsync -avz ./backend/uploads s3://your-bucket/menufy-uploads/
```

## Troubleshooting

### Common Issues

1. **Port already in use**
   ```bash
   sudo lsof -i :5000
   sudo kill -9 PID
   ```

2. **Database connection failed**
   - Check docker network
   - Verify credentials
   - Check firewall rules

3. **Out of memory**
   - Increase server RAM
   - Add swap space
   - Optimize Docker memory limits

### Recovery

**Restore Database**
```bash
gunzip < backup.sql.gz | docker exec -i menufy-db-prod psql -U menufy_user menufy_prod
```

## Post-Deployment Checklist

- [ ] SSL certificates installed and auto-renewal configured
- [ ] Database backups scheduled
- [ ] Monitoring configured
- [ ] Firewall rules configured
- [ ] Environment variables secured
- [ ] SMTP configured and tested
- [ ] Domain DNS records updated
- [ ] Application logs reviewed
- [ ] Performance tested
- [ ] Security scan completed

## Support

For deployment issues:
- Check logs: `docker-compose logs`
- Review documentation
- Open GitHub issue
