# 🍽️ Menufy - Complete Restaurant Menu Management System

> A full-stack, multi-language restaurant menu management platform with QR code generation and role-based access control.

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![Nuxt](https://img.shields.io/badge/Nuxt-3.x-00DC82)](https://nuxt.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-14+-blue)](https://www.postgresql.org/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

---

## ✨ Features

### 🔐 Role-Based Access Control
- **Admin Panel** - System administration and restaurant management
- **Restaurant Owner Dashboard** - Complete menu management
- **Public Menu** - Customer-facing menu via QR code

### 📱 Restaurant Owner Capabilities
- ✅ Create and manage categories
- ✅ Create subcategories (nested structure)
- ✅ Add menu items with photos (drag & drop upload)
- ✅ Generate QR codes for tables
- ✅ Create reusable menu templates
- ✅ Multi-language support (EN/AR)
- ✅ Real-time search and pagination

### 👨‍💼 Admin Capabilities
- ✅ Manage all restaurants (CRUD)
- ✅ Manage all users (CRUD)
- ✅ Assign roles
- ✅ View system analytics
- ✅ Monitor activity across restaurants

### 🌐 Multi-Language Support
- English (EN) - LTR
- Arabic (AR) - RTL with full layout mirroring
- Database-level translations (JSONB)
- Dynamic language switching

### 🎨 Professional UI/UX
- Modern, responsive design
- Mobile-first approach
- Tailwind CSS styling
- Loading states & animations
- Empty states & error handling
- Modal dialogs
- Toast notifications

---

## 🚀 Quick Start

### Prerequisites
```bash
- Node.js 18+
- .NET 8 SDK
- PostgreSQL 14+
```

### 1. Clone Repository
```bash
git clone <repository-url>
cd menufy
```

### 2. Setup Database
```bash
# Create database
createdb menufy_db

# Update connection string in:
# backend/src/Menufy.API/appsettings.json

# Run migrations
cd backend/src/Menufy.API
dotnet ef database update
```

### 3. Start Backend
```bash
cd backend/src/Menufy.API
dotnet restore
dotnet run
# Runs on: https://localhost:7001
```

### 4. Start Frontend
```bash
cd frontend
npm install
npm run dev
# Runs on: http://localhost:3000
```

### 5. Access Application
- **Admin Panel**: http://localhost:3000/admin
- **Owner Dashboard**: http://localhost:3000/dashboard
- **Public Menu**: http://localhost:3000/menu/{slug}

---

## 📚 Documentation

- **[Quick Start Guide](QUICK_START_GUIDE.md)** - Step-by-step setup and usage
- **[Implementation Status](IMPLEMENTATION_STATUS.md)** - Complete feature checklist
- **[System Architecture](SYSTEM_ARCHITECTURE.md)** - Technical architecture details

---

## 🏗️ Architecture

### Backend (.NET 8)
```
Clean Architecture + CQRS Pattern

├── API Layer (Controllers)
├── Application Layer (Commands, Queries, Handlers)
├── Domain Layer (Entities, Enums)
└── Infrastructure Layer (DbContext, Repositories)
```

### Frontend (Nuxt.js 3)
```
Vue 3 + TypeScript + Tailwind CSS

├── Pages (Routes)
├── Components (UI Library)
├── Stores (Pinia State Management)
├── Composables (Reusable Logic)
└── Layouts (Admin, Dashboard, Default)
```

### Database (PostgreSQL)
```
Entities:
├── User (with roles)
├── Restaurant
├── MenuCategory (with subcategories)
├── MenuItem (with translations)
├── QRCode
└── MenuTemplate
```

---

## 🔐 Security

- JWT-based authentication
- Password hashing (BCrypt)
- Role-based authorization
- Route protection (middleware)
- API endpoint protection
- Data isolation per restaurant
- CORS configuration
- SQL injection prevention

---

## 🌍 Internationalization

### Supported Languages
- **English (EN)** - Default, LTR
- **Arabic (AR)** - RTL with layout mirroring

### How It Works
1. Frontend: Translation files (`locales/en.json`, `locales/ar.json`)
2. Backend: JSONB columns for entity translations
3. API: `Accept-Language` header support
4. UI: Dynamic RTL/LTR switching

---

## 📊 API Endpoints

### Authentication
```
POST   /api/auth/register
POST   /api/auth/login
POST   /api/auth/refresh-token
```

### Restaurants (Admin)
```
GET    /api/restaurants
POST   /api/restaurants
PUT    /api/restaurants/{id}
DELETE /api/restaurants/{id}
```

### Categories (Owner + Admin)
```
GET    /api/restaurants/{restaurantId}/categories
POST   /api/restaurants/{restaurantId}/categories
PUT    /api/categories/{id}
DELETE /api/categories/{id}
```

### Menu Items (Owner + Admin)
```
POST   /api/categories/{categoryId}/items
PUT    /api/items/{id}
DELETE /api/items/{id}
```

### QR Codes (Owner + Admin)
```
GET    /api/qrcode/{restaurantId}
```

### Templates (Owner + Admin)
```
GET    /api/menu-templates
POST   /api/menu-templates
PUT    /api/menu-templates/{id}
DELETE /api/menu-templates/{id}
```

### Public Menu
```
GET    /api/menu/{slug}
```

---

## 🎯 User Roles

### Admin
- Access: `/admin` only
- Can manage all restaurants and users
- Cannot access restaurant owner dashboard

### Restaurant Owner
- Access: `/dashboard` only
- Can manage their own menu (categories, items)
- Can generate QR codes
- Can create templates
- Cannot see other restaurants' data

### Customer
- Access: `/menu/{slug}` (public)
- Can view menu via QR code
- Can switch languages

---

## 🔄 Typical Workflow

1. **Admin** creates a restaurant
2. **Admin** creates a restaurant owner user and assigns restaurant
3. **Owner** logs in to dashboard
4. **Owner** creates categories (e.g., Appetizers, Main Courses)
5. **Owner** creates subcategories (e.g., Pasta under Main Courses)
6. **Owner** adds items with photos and prices
7. **Owner** generates QR code
8. **Owner** prints and places QR code on tables
9. **Customer** scans QR code
10. **Customer** views menu with photos and prices

---

## 🛠️ Tech Stack

### Frontend
- **Framework**: Nuxt.js 3 (Vue 3)
- **Language**: TypeScript
- **Styling**: Tailwind CSS
- **State Management**: Pinia
- **HTTP Client**: Axios
- **i18n**: @nuxtjs/i18n

### Backend
- **Framework**: .NET 8
- **Architecture**: Clean Architecture + CQRS
- **ORM**: Entity Framework Core
- **Validation**: FluentValidation
- **Mediator**: MediatR
- **Authentication**: JWT Bearer

### Database
- **RDBMS**: PostgreSQL 14+
- **Migrations**: EF Core Migrations

### DevOps
- **Version Control**: Git
- **Package Managers**: npm, NuGet
- **Build Tools**: Vite, MSBuild

---

## 📦 Project Structure

```
menufy/
├── backend/
│   └── src/
│       ├── Menufy.API/              # API Controllers
│       ├── Menufy.Application/      # Business Logic (CQRS)
│       ├── Menufy.Domain/           # Entities & Interfaces
│       └── Menufy.Infrastructure/   # Data Access & Services
│
├── frontend/
│   ├── components/                  # Vue Components
│   ├── composables/                 # Reusable Logic
│   ├── layouts/                     # Page Layouts
│   ├── locales/                     # Translation Files
│   ├── middleware/                  # Route Guards
│   ├── pages/                       # Routes
│   ├── plugins/                     # Nuxt Plugins
│   ├── stores/                      # Pinia Stores
│   └── nuxt.config.ts              # Nuxt Configuration
│
├── IMPLEMENTATION_STATUS.md         # Feature Checklist
├── QUICK_START_GUIDE.md            # Setup Guide
├── SYSTEM_ARCHITECTURE.md          # Architecture Details
└── README.md                        # This file
```

---

## 🧪 Testing

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

---

## 🚀 Deployment

### Production Build

**Backend:**
```bash
cd backend/src/Menufy.API
dotnet publish -c Release -o ./publish
```

**Frontend:**
```bash
cd frontend
npm run build
npm run preview
```

### Environment Variables

**Backend** (`appsettings.Production.json`):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-production-db-connection"
  },
  "JwtSettings": {
    "Secret": "your-production-secret",
    "Issuer": "your-domain",
    "Audience": "your-domain"
  },
  "AppSettings": {
    "BaseUrl": "https://your-domain.com"
  }
}
```

**Frontend** (`.env.production`):
```
NUXT_PUBLIC_API_BASE_URL=https://api.your-domain.com
NUXT_PUBLIC_DEFAULT_CURRENCY=USD
```

---

## 📈 Performance

- **Frontend**: Code splitting, lazy loading, SSR
- **Backend**: Query optimization, indexing, caching
- **Database**: JSONB for fast translations, proper indexes

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- Built with ❤️ using modern web technologies
- Icons from Heroicons
- UI inspiration from Tailwind UI

---

## 📞 Support

For issues, questions, or feature requests:
- 📧 Email: support@menufy.com
- 🐛 Issues: [GitHub Issues](https://github.com/your-repo/issues)
- 📖 Docs: [Documentation](https://docs.menufy.com)

---

## ✅ Status

**Current Version**: 1.0.0
**Status**: ✅ Production Ready
**Last Updated**: November 11, 2024

### All Features Implemented ✅
- [x] Admin panel with full restaurant/user management
- [x] Restaurant owner dashboard with menu management
- [x] Category and subcategory support
- [x] Menu items with photo upload
- [x] QR code generation (one per restaurant)
- [x] Template builder and menu generation
- [x] Multi-language support (EN/AR)
- [x] RTL layout support
- [x] Search and pagination
- [x] Professional UI/UX
- [x] Complete API with all endpoints
- [x] Role-based security
- [x] Data isolation

---

**Ready to transform your restaurant's menu experience! 🎉**
