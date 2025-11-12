# 🏗️ Menu Generator Architecture Flow

## 📊 System Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                         MENUFY SYSTEM                            │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌──────────────┐    ┌──────────────┐    ┌──────────────┐      │
│  │   Frontend   │◄──►│   Backend    │◄──►│  Database    │      │
│  │   (Nuxt.js)  │    │   (.NET 8)   │    │ (SQL Server) │      │
│  └──────────────┘    └──────────────┘    └──────────────┘      │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🔄 Complete Flow Diagram

### **1. Template Creation Flow**

```
┌─────────────────────────────────────────────────────────────────┐
│ STEP 1: Restaurant Owner Creates Template                       │
└─────────────────────────────────────────────────────────────────┘

Frontend (dashboard/templates.vue)
    │
    │ 1. Owner clicks "Create Template"
    │
    ▼
┌────────────────────────────────────┐
│ Modal Opens                        │
│ ┌────────────────────────────────┐ │
│ │ Name: "Italian Restaurant"     │ │
│ │ Description: "..."             │ │
│ │                                │ │
│ │ ┌──────────────────────────┐   │ │
│ │ │ ThemeCustomizer          │   │ │
│ │ │ - Colors                 │   │ │
│ │ │ - Fonts                  │   │ │
│ │ │ - Layout                 │   │ │
│ │ └──────────────────────────┘   │ │
│ │                                │ │
│ │ ┌──────────────────────────┐   │ │
│ │ │ Categories & Items       │   │ │
│ │ │ - Appetizers             │   │ │
│ │ │   - Bruschetta ($8.50)   │   │ │
│ │ │ - Main Courses           │   │ │
│ │ │   - Pasta ($15.00)       │   │ │
│ │ └──────────────────────────┘   │ │
│ └────────────────────────────────┘ │
└────────────────────────────────────┘
    │
    │ 2. Click "Create"
    │
    ▼
POST /api/menu-templates
    │
    │ 3. Request Body:
    │    {
    │      name: "Italian Restaurant",
    │      theme: { primaryColor: "#dc2626", ... },
    │      structure: { categories: [...] }
    │    }
    │
    ▼
Backend (CreateMenuTemplateCommandHandler)
    │
    │ 4. Validate data
    │ 5. Serialize theme & structure to JSON
    │ 6. Save to database
    │
    ▼
Database (MenuTemplates table)
    │
    │ 7. Record saved:
    │    Id: guid
    │    Name: "Italian Restaurant"
    │    Theme: "{...json...}"
    │    Structure: "{...json...}"
    │    RestaurantId: guid
    │
    ▼
Response to Frontend
    │
    │ 8. Success message
    │ 9. Template appears in list
    │
    ▼
✅ Template Created!
```

---

### **2. Apply Template to Restaurant Flow**

```
┌─────────────────────────────────────────────────────────────────┐
│ STEP 2: Apply Template to Generate Menu                         │
└─────────────────────────────────────────────────────────────────┘

Frontend (dashboard/settings.vue)
    │
    │ 1. Owner selects template from dropdown
    │ 2. Clicks "Apply Template"
    │
    ▼
POST /api/restaurants/{id}/apply-template
    │
    │ 3. Request Body:
    │    {
    │      templateId: "template-guid",
    │      overwriteExisting: false
    │    }
    │
    ▼
Backend (ApplyTemplateCommandHandler)
    │
    │ 4. Validate restaurant & template
    │
    ▼
    │ 5. Call GenerateMenuCommand
    │
    ▼
GenerateMenuCommandHandler
    │
    │ 6. Parse template structure JSON
    │ 7. For each category in template:
    │    - Create MenuCategory entity
    │    - Set translations
    │    - Set display order
    │
    │ 8. For each item in category:
    │    - Create MenuItem entity
    │    - Set translations
    │    - Set price, image, etc.
    │
    ▼
Database Transaction
    │
    │ 9. Insert categories
    │ 10. Insert items
    │ 11. Update Restaurant.ActiveTemplateId
    │ 12. Update Template.UsageCount++
    │
    ▼
Response to Frontend
    │
    │ 13. Success:
    │     {
    │       categoriesCreated: 5,
    │       itemsCreated: 25,
    │       appliedAt: "2024-..."
    │     }
    │
    ▼
✅ Menu Generated!
```

---

### **3. Customize Restaurant Settings Flow**

```
┌─────────────────────────────────────────────────────────────────┐
│ STEP 3: Customize Theme & Display Settings                      │
└─────────────────────────────────────────────────────────────────┘

Frontend (dashboard/settings.vue)
    │
    │ 1. Owner customizes:
    │    - Primary color → #059669 (green)
    │    - Layout → Grid
    │    - Show images → true
    │    - Currency → EUR
    │
    ▼
PUT /api/restaurants/{id}/settings
    │
    │ 2. Request Body:
    │    {
    │      customTheme: {
    │        primaryColor: "#059669",
    │        layout: "grid"
    │      },
    │      displaySettings: {
    │        showImages: true,
    │        enableSearch: true
    │      },
    │      currency: "EUR"
    │    }
    │
    ▼
Backend (UpdateRestaurantSettingsCommandHandler)
    │
    │ 3. Load restaurant from DB
    │ 4. Serialize customTheme to JSON
    │ 5. Serialize displaySettings to JSON
    │ 6. Update restaurant properties:
    │    - CustomTheme = "{...json...}"
    │    - MenuDisplaySettings = "{...json...}"
    │    - Currency = "EUR"
    │
    ▼
Database (Restaurants table)
    │
    │ 7. Update record:
    │    CustomTheme: "{primaryColor: '#059669', ...}"
    │    MenuDisplaySettings: "{showImages: true, ...}"
    │    Currency: "EUR"
    │    UpdatedAt: DateTime.UtcNow
    │
    ▼
Response to Frontend
    │
    │ 8. Success with updated settings
    │
    ▼
✅ Settings Saved!
```

---

### **4. Customer Views Public Menu Flow**

```
┌─────────────────────────────────────────────────────────────────┐
│ STEP 4: Customer Scans QR Code & Views Menu                     │
└─────────────────────────────────────────────────────────────────┘

Customer
    │
    │ 1. Scans QR Code
    │    URL: https://menufy.com/menu/my-restaurant
    │
    ▼
Frontend (pages/menu/[slug].vue)
    │
    │ 2. Page loads
    │ 3. onMounted() triggered
    │
    ▼
GET /api/menu/my-restaurant?lang=en
    │
    ▼
Backend (GetPublicMenuQueryHandler)
    │
    │ 4. Find restaurant by slug
    │ 5. Load categories & items
    │ 6. Load ActiveTemplate (if set)
    │ 7. Load CustomTheme (if set)
    │ 8. Load MenuDisplaySettings
    │
    │ 9. Determine theme priority:
    │    - CustomTheme (highest)
    │    - ActiveTemplate.Theme
    │    - Default theme (fallback)
    │
    │ 10. Build response:
    │     {
    │       restaurantName: "My Restaurant",
    │       theme: {
    │         primaryColor: "#059669",
    │         layout: "grid",
    │         ...
    │       },
    │       displaySettings: {
    │         showImages: true,
    │         enableSearch: true,
    │         ...
    │       },
    │       currency: "EUR",
    │       categories: [...]
    │     }
    │
    │ 11. Increment TotalMenuViews++
    │
    ▼
Response to Frontend
    │
    │ 12. menu.value = response.data
    │
    ▼
Render Menu with Theme
    │
    │ 13. Apply CSS variables:
    │     --primary-color: #059669
    │     --layout: grid
    │
    │ 14. Render based on layout:
    │     - Grid → 2-3 columns
    │     - List → Single column
    │     - Cards → Card-based layout
    │
    │ 15. Apply display settings:
    │     - Show/hide images
    │     - Show/hide prices
    │     - Enable search bar
    │
    ▼
┌────────────────────────────────────┐
│ Beautiful Themed Menu Displayed!   │
│                                    │
│ [Logo]  My Restaurant              │
│ ────────────────────────────────   │
│                                    │
│ 🔍 Search...                       │
│                                    │
│ ┌──────┐ ┌──────┐ ┌──────┐        │
│ │ 🍝   │ │ 🍕   │ │ 🥗   │        │
│ │Pasta │ │Pizza │ │Salad │        │
│ │€15   │ │€12   │ │€8    │        │
│ └──────┘ └──────┘ └──────┘        │
│                                    │
└────────────────────────────────────┘
    │
    ▼
✅ Customer Enjoys Menu!
```

---

## 🗄️ Database Schema

```
┌─────────────────────────────────────────────────────────────────┐
│                         DATABASE TABLES                          │
└─────────────────────────────────────────────────────────────────┘

┌──────────────────────────┐
│ MenuTemplates            │
├──────────────────────────┤
│ Id (PK)                  │
│ Name                     │
│ Description              │
│ RestaurantId (FK)        │◄──┐
│ Structure (JSON)         │   │
│ Theme (JSON)             │   │
│ Tags (JSON)              │   │
│ IsPublished              │   │
│ UsageCount               │   │
│ LastUsedAt               │   │
│ CreatedAt                │   │
│ UpdatedAt                │   │
└──────────────────────────┘   │
                               │
                               │
┌──────────────────────────┐   │
│ Restaurants              │   │
├──────────────────────────┤   │
│ Id (PK)                  │───┘
│ Name                     │
│ Slug                     │
│ OwnerId (FK)             │
│ ActiveTemplateId (FK)    │───┐
│ CustomTheme (JSON)       │   │ NEW
│ MenuDisplaySettings      │   │ NEW
│ Currency                 │   │ NEW
│ DefaultLanguage          │   │ NEW
│ TotalMenuViews           │   │ NEW
│ LastMenuUpdate           │   │ NEW
│ CreatedAt                │   │
│ UpdatedAt                │   │
└──────────────────────────┘   │
        │                      │
        │                      │
        ▼                      │
┌──────────────────────────┐   │
│ MenuCategories           │   │
├──────────────────────────┤   │
│ Id (PK)                  │   │
│ Name                     │   │
│ Translations (JSON)      │   │
│ DisplayOrder             │   │
│ RestaurantId (FK)        │   │
│ ParentCategoryId (FK)    │   │
│ CreatedAt                │   │
│ UpdatedAt                │   │
└──────────────────────────┘   │
        │                      │
        │                      │
        ▼                      │
┌──────────────────────────┐   │
│ MenuItems                │   │
├──────────────────────────┤   │
│ Id (PK)                  │   │
│ Name                     │   │
│ Description              │   │
│ Translations (JSON)      │   │
│ Price                    │   │
│ ImageUrl                 │   │
│ IsAvailable              │   │
│ DisplayOrder             │   │
│ CategoryId (FK)          │   │
│ CreatedAt                │   │
│ UpdatedAt                │   │
└──────────────────────────┘   │
                               │
                               │
┌──────────────────────────┐   │
│ Users                    │   │
├──────────────────────────┤   │
│ Id (PK)                  │   │
│ Name                     │   │
│ Email                    │   │
│ PasswordHash             │   │
│ Role                     │   │
│ CreatedAt                │   │
│ UpdatedAt                │   │
└──────────────────────────┘   │
                               │
                               │
                               └───────────┐
                                           │
┌──────────────────────────┐               │
│ QRCodes                  │               │
├──────────────────────────┤               │
│ Id (PK)                  │               │
│ ImageUrl                 │               │
│ Link                     │               │
│ RestaurantId (FK)        │───────────────┘
│ CreatedAt                │
│ UpdatedAt                │
└──────────────────────────┘
```

---

## 🔌 API Endpoints Map

```
┌─────────────────────────────────────────────────────────────────┐
│                         API ENDPOINTS                            │
└─────────────────────────────────────────────────────────────────┘

Authentication
├── POST   /api/auth/register
├── POST   /api/auth/login
└── POST   /api/auth/refresh

Templates (Authenticated)
├── GET    /api/menu-templates
│   └─► List all templates (global + user's)
├── GET    /api/menu-templates/{id}
│   └─► Get template details
├── POST   /api/menu-templates
│   └─► Create new template
├── PUT    /api/menu-templates/{id}
│   └─► Update template
└── DELETE /api/menu-templates/{id}
    └─► Delete template

Restaurant Settings (Authenticated) ⭐ NEW
├── GET    /api/restaurants/{id}/settings
│   └─► Get current settings
├── PUT    /api/restaurants/{id}/settings
│   └─► Update settings
└── POST   /api/restaurants/{id}/apply-template
    └─► Apply template to restaurant

Menu Generation (Authenticated)
└── POST   /api/restaurants/{id}/generate-menu
    └─► Generate menu from template

Categories (Authenticated)
├── GET    /api/restaurants/{id}/categories
├── POST   /api/restaurants/{id}/categories
├── PUT    /api/categories/{id}
└── DELETE /api/categories/{id}

Items (Authenticated)
├── POST   /api/categories/{id}/items
├── PUT    /api/items/{id}
└── DELETE /api/items/{id}

Public Menu (No Auth)
└── GET    /api/menu/{slug}?lang=en
    └─► Get public menu with theme ⭐ ENHANCED

QR Codes (Authenticated)
├── GET    /api/restaurants/{id}/qr-code
└── POST   /api/restaurants/{id}/qr-code
```

---

## 🎨 Theme Application Flow

```
┌─────────────────────────────────────────────────────────────────┐
│ How Theme is Applied to Public Menu                             │
└─────────────────────────────────────────────────────────────────┘

Backend Determines Theme:
    │
    ├─► 1. Check Restaurant.CustomTheme
    │   └─► If exists → Use this (HIGHEST PRIORITY)
    │
    ├─► 2. Check Restaurant.ActiveTemplate.Theme
    │   └─► If exists → Use this (MEDIUM PRIORITY)
    │
    └─► 3. Use Default Theme
        └─► Fallback (LOWEST PRIORITY)

    ▼

Send to Frontend:
{
  theme: {
    primaryColor: "#059669",
    accentColor: "#10b981",
    layout: "grid",
    cardStyle: "modern",
    ...
  }
}

    ▼

Frontend Applies Theme:

1. CSS Variables:
   <div :style="{
     '--primary': theme.primaryColor,
     '--accent': theme.accentColor
   }">

2. Dynamic Classes:
   <div :class="[
     theme.layout === 'grid' ? 'grid grid-cols-3' : 'flex flex-col',
     theme.cardStyle === 'modern' ? 'rounded-xl shadow-lg' : 'rounded border'
   ]">

3. Conditional Rendering:
   <img v-if="theme.showImages" :src="item.imageUrl" />
   <span v-if="displaySettings.showPrices">{{ item.price }}</span>

    ▼

Result: Fully Themed Menu! 🎨
```

---

## 📱 User Roles & Permissions

```
┌─────────────────────────────────────────────────────────────────┐
│                         USER ROLES                               │
└─────────────────────────────────────────────────────────────────┘

┌──────────────────────────┐
│ Admin                    │
├──────────────────────────┤
│ ✅ Create global templates│
│ ✅ Manage all restaurants │
│ ✅ View analytics         │
│ ✅ Delete any data        │
└──────────────────────────┘

┌──────────────────────────┐
│ Restaurant Owner         │
├──────────────────────────┤
│ ✅ Create templates       │
│ ✅ Manage own restaurant  │
│ ✅ Customize theme        │
│ ✅ Generate menus         │
│ ✅ View own analytics     │
│ ❌ Access other restaurants│
└──────────────────────────┘

┌──────────────────────────┐
│ Customer (Public)        │
├──────────────────────────┤
│ ✅ View public menus      │
│ ✅ Switch languages       │
│ ✅ Search items           │
│ ❌ Edit anything          │
└──────────────────────────┘
```

---

## 🚀 Deployment Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                    PRODUCTION DEPLOYMENT                         │
└─────────────────────────────────────────────────────────────────┘

                    ┌──────────────┐
                    │   Internet   │
                    └──────┬───────┘
                           │
                           ▼
                    ┌──────────────┐
                    │  CloudFlare  │
                    │     CDN      │
                    └──────┬───────┘
                           │
                ┌──────────┴──────────┐
                │                     │
                ▼                     ▼
        ┌───────────────┐     ┌───────────────┐
        │   Frontend    │     │   Backend     │
        │   (Vercel)    │     │  (Azure App)  │
        │   Nuxt.js     │     │   .NET 8 API  │
        └───────────────┘     └───────┬───────┘
                                      │
                                      ▼
                              ┌───────────────┐
                              │   Database    │
                              │ (Azure SQL)   │
                              └───────────────┘
                                      │
                                      ▼
                              ┌───────────────┐
                              │  Blob Storage │
                              │   (Images)    │
                              └───────────────┘
```

---

## 📊 Performance Considerations

```
┌─────────────────────────────────────────────────────────────────┐
│                    PERFORMANCE OPTIMIZATIONS                     │
└─────────────────────────────────────────────────────────────────┘

Backend:
├── Database Indexing
│   ├── Index on Restaurants.Slug (for public menu lookup)
│   ├── Index on MenuCategories.RestaurantId
│   └── Index on MenuItems.CategoryId
│
├── Caching
│   ├── Redis cache for public menus (5 min TTL)
│   ├── Cache templates list
│   └── Cache restaurant settings
│
└── Query Optimization
    ├── Use AsNoTracking() for read-only queries
    ├── Include() for eager loading
    └── Pagination for large lists

Frontend:
├── Image Optimization
│   ├── Use Next/Image component
│   ├── Lazy loading
│   └── WebP format
│
├── Code Splitting
│   ├── Route-based splitting
│   └── Component lazy loading
│
└── State Management
    ├── Pinia stores with persistence
    └── Optimistic updates
```

---

This architecture provides a complete view of how the enhanced menu generator system works! 🎉

