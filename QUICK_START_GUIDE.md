# Menufy - Quick Start Guide

## 🎯 System Overview

**Menufy** is a complete restaurant menu management system with:
- **Admin Panel** - For system administrators
- **Restaurant Dashboard** - For restaurant owners
- **Public Menu** - For customers (QR code access)

---

## 🚀 Getting Started

### Prerequisites
- Node.js 18+ and npm
- .NET 8 SDK
- PostgreSQL 14+

### 1. Database Setup

```bash
# Create PostgreSQL database
createdb menufy_db

# Update connection string in backend/src/Menufy.API/appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=menufy_db;Username=postgres;Password=yourpassword"
}

# Run migrations
cd backend/src/Menufy.API
dotnet ef database update
```

### 2. Backend Setup

```bash
cd backend/src/Menufy.API
dotnet restore
dotnet run
# Backend runs on: https://localhost:7001
```

### 3. Frontend Setup

```bash
cd frontend
npm install
npm run dev
# Frontend runs on: http://localhost:3000
```

---

## 👥 User Roles & Access

### 1. Admin Role
**Access**: `/admin` only
**Can**:
- View system analytics
- Manage all restaurants (create, edit, delete)
- Manage all users (create, edit, delete, assign roles)
- View all data across the system

**Cannot**:
- Access restaurant owner dashboard
- Create menu items directly

### 2. Restaurant Owner Role
**Access**: `/dashboard` only
**Can**:
- Manage their restaurant's categories
- Create subcategories
- Add/edit/delete menu items
- Upload item photos
- Generate QR code
- Create menu templates
- View their restaurant stats

**Cannot**:
- Access admin panel
- See other restaurants' data
- Manage users

### 3. Customer Role
**Access**: `/menu/{slug}` (public)
**Can**:
- View restaurant menu via QR code
- Switch languages (EN/AR)

---

## 📝 Complete Workflow

### Step 1: Admin Setup
1. Register first user → automatically becomes Admin
2. Login to `/admin`
3. Create a new restaurant:
   - Name: "My Restaurant"
   - Slug: "my-restaurant"
   - Contact info
   - Logo (optional)

### Step 2: Create Restaurant Owner
1. In Admin Panel → Users
2. Click "Create User"
3. Fill details:
   - Name, Email, Password
   - Role: **RestaurantOwner**
   - Assign Restaurant: Select the restaurant
4. Save

### Step 3: Restaurant Owner Login
1. Owner logs in with their credentials
2. Redirected to `/dashboard`
3. See their restaurant dashboard

### Step 4: Build Menu
1. Go to "Categories" page
2. Create main categories:
   ```
   - Appetizers
   - Main Courses
   - Desserts
   - Beverages
   ```
3. Create subcategories (optional):
   ```
   Main Courses
   ├── Pasta
   ├── Grilled
   └── Seafood
   ```
4. Add items to each category:
   - Click "Add Item" button
   - Fill form:
     * Name
     * Description
     * Price
     * Upload photo (drag & drop)
     * Set availability
   - Save

### Step 5: Generate QR Code
1. Go to "QR Codes" page
2. Click "Generate QR Code"
3. Download QR code image
4. Print and place on restaurant tables

### Step 6: Customer Access
1. Customer scans QR code
2. Opens: `https://yoursite.com/menu/my-restaurant`
3. Views full menu with:
   - Categories
   - Items with photos
   - Prices
   - Descriptions
4. Can switch language (EN ↔ AR)

---

## 🎨 Using Templates

### Create Template
1. Go to "Templates" page
2. Click "Create Template"
3. Define structure:
   - Template name
   - Categories with items
   - Translations (EN/AR)
   - Theme colors
   - Tags
4. Save template

### Generate Menu from Template
1. Go to Dashboard
2. Click "Generate Sample Menu"
3. Select template
4. Click "Generate"
5. Menu is created instantly!

---

## 🔐 API Endpoints Reference

### Authentication
```
POST /api/auth/register
POST /api/auth/login
POST /api/auth/refresh-token
```

### Restaurants (Admin only)
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
GET /api/qrcode/{restaurantId}
```

### Templates (Owner + Admin)
```
GET    /api/menu-templates
POST   /api/menu-templates
PUT    /api/menu-templates/{id}
DELETE /api/menu-templates/{id}
```

### Public Menu (No auth)
```
GET /api/menu/{slug}
```

---

## 🌐 Multi-Language Support

### Supported Languages
- English (EN) - Default
- Arabic (AR) - RTL support

### How it Works
1. User clicks language switcher
2. UI instantly changes
3. Menu items show translated names/descriptions
4. Layout switches to RTL for Arabic

### Adding Translations
When creating categories/items, provide translations:
```json
{
  "name": "Pizza",
  "translations": {
    "en": "Pizza",
    "ar": "بيتزا"
  }
}
```

---

## 📱 Features Checklist

### Admin Panel ✅
- [x] Dashboard with analytics
- [x] Restaurant management (CRUD)
- [x] User management (CRUD)
- [x] Role assignment
- [x] System-wide statistics

### Restaurant Dashboard ✅
- [x] Category management (CRUD)
- [x] Subcategory support
- [x] Menu item management (CRUD)
- [x] Photo upload (drag & drop)
- [x] Search & pagination
- [x] QR code generation
- [x] Template builder
- [x] Menu generation from templates
- [x] Multi-language support

### Public Menu ✅
- [x] View menu by QR scan
- [x] Responsive design
- [x] Language switcher
- [x] RTL support
- [x] Category tree view
- [x] Item images
- [x] Prices with currency

### Security ✅
- [x] JWT authentication
- [x] Role-based access
- [x] Password hashing
- [x] Route protection
- [x] API authorization
- [x] Data isolation

---

## 🐛 Troubleshooting

### Backend not starting
```bash
# Check .NET version
dotnet --version  # Should be 8.0+

# Check database connection
# Update appsettings.json with correct PostgreSQL credentials

# Run migrations
dotnet ef database update
```

### Frontend not loading
```bash
# Clear cache
rm -rf .nuxt node_modules
npm install
npm run dev
```

### Login issues
- Check JWT secret in appsettings.json
- Verify user role is correct
- Clear browser localStorage
- Check backend logs

### QR code not generating
- Verify BaseUrl in appsettings.json
- Check restaurant slug is unique
- Ensure restaurant is active

---

## 📞 Support

For issues or questions:
1. Check this guide
2. Review IMPLEMENTATION_STATUS.md
3. Check backend logs
4. Check browser console

---

## 🎉 You're Ready!

Your complete restaurant menu management system is now running!

**Admin Panel**: http://localhost:3000/admin
**Owner Dashboard**: http://localhost:3000/dashboard
**Public Menu**: http://localhost:3000/menu/{slug}

Happy menu building! 🍕🍔🍜

