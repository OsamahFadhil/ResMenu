# Menufy - Digital Menu Management Platform

Menufy is a full-stack online menu management platform for restaurants. Restaurants can register, create digital menus, and share them via QR codes. Customers can scan QR codes to view menus online.

## Features

- Restaurant registration and authentication
- Digital menu creation and management
- Menu categories and items with images and prices
- QR code generation for contactless menu access
- Public menu viewing (no app required for customers)
- Admin dashboard for restaurant management
- Responsive design for all devices

## Tech Stack

### Backend
- **Framework**: ASP.NET Core 8
- **Architecture**: Clean Architecture + CQRS
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Authentication**: JWT
- **Libraries**:
  - MediatR (CQRS)
  - FluentValidation
  - AutoMapper
  - QRCoder
  - Swashbuckle (Swagger)

### Frontend
- **Framework**: Nuxt.js 3 (Vue 3 Composition API)
- **UI**: TailwindCSS
- **State Management**: Pinia
- **HTTP Client**: Axios

### Infrastructure
- **Containerization**: Docker & Docker Compose
- **Database**: PostgreSQL 16
- **Database Management**: pgAdmin 4 (optional)

## Project Structure

```
menufy/
├── backend/
│   ├── src/
│   │   ├── Menufy.Domain/          # Domain entities and enums
│   │   ├── Menufy.Application/      # CQRS commands, queries, DTOs
│   │   ├── Menufy.Infrastructure/   # EF Core, services
│   │   └── Menufy.API/              # Controllers, Program.cs
│   ├── Dockerfile
│   └── Menufy.sln
├── frontend/
│   ├── assets/                      # CSS and static assets
│   ├── components/                  # Vue components
│   ├── composables/                 # Composables (useApi)
│   ├── pages/                       # Nuxt pages
│   ├── stores/                      # Pinia stores
│   ├── Dockerfile
│   ├── nuxt.config.ts
│   ├── tailwind.config.js
│   └── package.json
├── docker-compose.yml
└── README.md
```

## Getting Started

### Prerequisites

- Docker Desktop (includes Docker Compose)
- OR:
  - .NET 8 SDK
  - Node.js 20+
  - PostgreSQL 16

### Quick Start with Docker

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd menufy
   ```

2. **Start all services with Docker Compose**
   ```bash
   docker-compose up --build
   ```

3. **Access the application**
   - Frontend: http://localhost:3000
   - Backend API: http://localhost:5000
   - Swagger UI: http://localhost:5000/swagger
   - pgAdmin: http://localhost:5050

4. **pgAdmin Setup** (Optional)
   - Email: admin@menufy.com
   - Password: admin
   - Add server connection:
     - Host: db
     - Port: 5432
     - Database: menufy
     - Username: postgres
     - Password: postgres

### Manual Setup (Without Docker)

#### Backend Setup

1. **Navigate to backend directory**
   ```bash
   cd backend
   ```

2. **Configure database connection**
   - Copy `.env.sample` to `.env`
   - Update the connection string if needed

3. **Restore dependencies**
   ```bash
   dotnet restore
   ```

4. **Run migrations**
   ```bash
   cd src/Menufy.API
   dotnet ef database update
   ```

5. **Run the API**
   ```bash
   dotnet run
   ```

   API will be available at http://localhost:5000

#### Frontend Setup

1. **Navigate to frontend directory**
   ```bash
   cd frontend
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Configure environment**
   - Copy `.env.sample` to `.env`
   - Update `NUXT_PUBLIC_API_BASE` if needed

4. **Run the development server**
   ```bash
   npm run dev
   ```

   Frontend will be available at http://localhost:3000

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register new restaurant
- `POST /api/auth/login` - Login (returns JWT)

### Menus
- `GET /api/menu/{slug}` - Get public menu by restaurant slug
- `POST /api/restaurants/{id}/categories` - Create menu category
- `POST /api/categories/{id}/items` - Create menu item

### QR Codes
- `GET /api/qrcode/{restaurantId}` - Generate/Get QR code

## User Roles

1. **System Administrator** - Manages all restaurants
2. **Restaurant Owner** - Manages their restaurant and menus
3. **Customer** - Views menus (no authentication required)

## Environment Variables

### Backend (.env)

```env
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=menufy;Username=postgres;Password=postgres
JwtSettings__Secret=YourSuperSecretKeyThatIsAtLeast32CharactersLong!
JwtSettings__Issuer=MenufyAPI
JwtSettings__Audience=MenufyClient
AppSettings__BaseUrl=http://localhost:3000
```

### Frontend (.env)

```env
NUXT_PUBLIC_API_BASE=http://localhost:5000/api
```

## Database Schema

### Core Tables
- **Users** - User accounts (restaurant owners, admins)
- **Restaurants** - Restaurant profiles
- **MenuCategories** - Menu categories (Appetizers, Main Courses, etc.)
- **MenuItems** - Individual menu items
- **QRCodes** - Generated QR codes for restaurants

## Development Workflow

### Adding a New Feature (Backend)

1. **Create domain entity** in `Menufy.Domain/Entities`
2. **Create DTOs** in `Menufy.Application/Features/{Feature}/DTOs`
3. **Create command/query** in `Menufy.Application/Features/{Feature}/Commands` or `Queries`
4. **Create handler** implementing `IRequestHandler`
5. **Add validator** using FluentValidation
6. **Create controller** in `Menufy.API/Controllers`
7. **Add migration**:
   ```bash
   dotnet ef migrations add MigrationName --project src/Menufy.Infrastructure --startup-project src/Menufy.API
   ```

### Adding a New Page (Frontend)

1. **Create page** in `pages/` directory
2. **Create components** in `components/` if needed
3. **Use stores** for state management
4. **Use composables** for API calls

## Production Deployment

### Docker Production Build

```bash
docker-compose -f docker-compose.prod.yml up -d
```

### Security Considerations

1. **Change default JWT secret** in production
2. **Use environment variables** for all secrets
3. **Enable HTTPS** in production
4. **Configure CORS** appropriately
5. **Use strong database passwords**
6. **Enable rate limiting** on API endpoints

## Testing

### Backend Tests
```bash
cd backend
dotnet test
```

### Frontend Tests
```bash
cd frontend
npm run test
```

## Common Issues

### Database Connection Failed
- Ensure PostgreSQL is running
- Check connection string in configuration
- Verify database credentials

### CORS Errors
- Check `AllowAll` CORS policy in `Program.cs`
- Verify frontend is making requests to correct API URL

### Docker Build Issues
- Clear Docker cache: `docker-compose down -v`
- Rebuild: `docker-compose up --build --force-recreate`

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License.

## Support

For issues and questions:
- Create an issue in the repository
- Contact: support@menufy.com

## Roadmap

- [ ] Multi-language menu support
- [ ] Menu item image upload to cloud storage
- [ ] QR code scan analytics
- [ ] Menu themes and customization
- [ ] Table-specific QR codes
- [ ] Online ordering integration
- [ ] Customer reviews and ratings
- [ ] Dietary information and allergen warnings

---

Built with ❤️ using ASP.NET Core, Nuxt.js, and PostgreSQL
