# Menu Generator System - Complete Analysis & Implementation Guide

## 📋 Table of Contents
1. [Current System Overview](#current-system-overview)
2. [Architecture Analysis](#architecture-analysis)
3. [Template System Deep Dive](#template-system-deep-dive)
4. [Public Menu Display](#public-menu-display)
5. [Full Customization Implementation Plan](#full-customization-implementation-plan)
6. [Advanced Features Roadmap](#advanced-features-roadmap)

---

## 🎯 Current System Overview

### What Exists Now

#### **Backend (C# .NET)**
- ✅ Menu template entity system
- ✅ Template CRUD operations
- ✅ Menu generation from templates
- ✅ Built-in default template
- ✅ Multi-language support (translations)
- ✅ QR code generation linked to restaurant slug

#### **Frontend (Nuxt 3 + Vue 3)**
- ✅ Template management page (`/dashboard/templates`)
- ✅ Template creation/editing UI
- ✅ Menu generation trigger
- ✅ Public menu display (`/menu/[slug]`)
- ✅ Language switcher
- ✅ Responsive design

---

## 🏗️ Architecture Analysis

### Data Flow

```
┌─────────────────┐
│  Restaurant     │
│  Owner          │
└────────┬────────┘
         │
         ├─── Creates/Edits Templates
         │    (Categories + Items + Theme)
         │
         ├─── Generates Menu from Template
         │    (Applies template to restaurant)
         │
         └─── Generates QR Code
              (Links to /menu/{slug})
                     │
                     ▼
         ┌───────────────────────┐
         │  Customer Scans QR    │
         └───────────┬───────────┘
                     │
                     ▼
         ┌───────────────────────┐
         │  Public Menu Display  │
         │  - Restaurant Info    │
         │  - Categories Tree    │
         │  - Menu Items         │
         │  - Prices             │
         │  - Multi-language     │
         └───────────────────────┘
```

### Database Schema

```sql
-- Templates Table
MenuTemplates
  ├── Id (Guid)
  ├── Name (string)
  ├── Description (string?)
  ├── RestaurantId (Guid?) -- null = global template
  ├── IsGlobal (bool)
  ├── IsPublished (bool)
  ├── Tags (string[])
  ├── Theme (JSON)
  │   ├── primaryColor
  │   ├── accentColor
  │   ├── backgroundColor
  │   ├── surfaceColor
  │   ├── textColor
  │   └── fontFamily
  └── Structure (JSON)
      └── categories[]
          ├── name
          ├── translations{}
          ├── displayOrder
          ├── icon
          └── items[]
              ├── name
              ├── nameTranslations{}
              ├── description
              ├── descriptionTranslations{}
              ├── price
              ├── imageUrl
              ├── isAvailable
              ├── displayOrder
              └── tags[]

-- Generated Menu (Actual Data)
MenuCategories
  ├── Id
  ├── RestaurantId
  ├── Name
  ├── Translations (JSON)
  ├── DisplayOrder
  ├── ParentCategoryId (for nested categories)
  └── MenuItems[]
      ├── Id
      ├── CategoryId
      ├── Name
      ├── Description
      ├── Translations (JSON)
      ├── Price
      ├── ImageUrl
      ├── IsAvailable
      └── DisplayOrder
```

---

## 🎨 Template System Deep Dive

### Template Structure

#### **1. Theme Customization**
```typescript
interface TemplateTheme {
  primaryColor: string      // Main brand color
  accentColor: string       // Secondary/highlight color
  backgroundColor: string   // Page background
  surfaceColor: string      // Card/surface background
  textColor: string         // Main text color
  fontFamily: string        // Typography
}
```

**Current Default:**
```typescript
{
  primaryColor: '#f97316',    // Orange
  accentColor: '#facc15',     // Yellow
  backgroundColor: '#fff7ed', // Light orange
  surfaceColor: '#ffffff',    // White
  textColor: '#1f2937',       // Dark gray
  fontFamily: 'Inter'
}
```

#### **2. Category Structure**
```typescript
interface TemplateCategory {
  name: string                        // English name
  translations?: Record<string, string> // { ar: 'المقبلات', fr: 'Entrées' }
  displayOrder: number                // Sort order
  icon?: string                       // Emoji or icon
  items: TemplateItem[]              // Menu items
}
```

#### **3. Item Structure**
```typescript
interface TemplateItem {
  name: string
  nameTranslations?: Record<string, string>
  description?: string
  descriptionTranslations?: Record<string, string>
  price: number
  imageUrl?: string
  isAvailable: boolean
  displayOrder: number
  tags?: string[]          // ['vegetarian', 'spicy', 'popular']
  rating?: number          // Future: customer ratings
}
```

### Template Types

#### **1. Global Templates** (isGlobal: true)
- Available to all restaurants
- Created by admins
- Pre-built industry templates:
  - Italian Restaurant
  - Fast Food
  - Café & Bakery
  - Fine Dining
  - Asian Cuisine
  - etc.

#### **2. Restaurant Templates** (isGlobal: false)
- Created by restaurant owners
- Private to their restaurant
- Reusable for seasonal menus
- Can be duplicated and modified

---

## 📱 Public Menu Display

### Current Implementation

**Route:** `/menu/[slug]`

**Features:**
- ✅ Restaurant header with logo
- ✅ Contact information
- ✅ Language switcher
- ✅ Category tree display
- ✅ Item cards with images
- ✅ Price formatting
- ✅ Responsive design
- ✅ Loading states
- ✅ Error handling

### Display Flow

```
1. Customer scans QR code
   ↓
2. Opens /menu/{restaurant-slug}?lang=en
   ↓
3. Frontend fetches: GET /api/menu/{slug}?lang=en
   ↓
4. Backend returns:
   - Restaurant info (name, logo, contact)
   - Categories tree (with translations)
   - Menu items (filtered by isAvailable)
   ↓
5. Frontend renders:
   - Restaurant header
   - Category sections
   - Item cards
   - Prices in local currency
```

---

## 🚀 Full Customization Implementation Plan

### Phase 1: Enhanced Template System

#### **1.1 Add More Theme Options**

**File:** `frontend/stores/templates.ts`

```typescript
export interface TemplateTheme {
  // Colors
  primaryColor: string
  accentColor: string
  backgroundColor: string
  surfaceColor: string
  textColor: string
  
  // NEW: Typography
  fontFamily: string
  headingFont?: string
  bodyFont?: string
  fontSize: 'small' | 'medium' | 'large'
  
  // NEW: Layout
  layout: 'grid' | 'list' | 'cards'
  cardStyle: 'modern' | 'classic' | 'minimal'
  borderRadius: 'none' | 'small' | 'medium' | 'large'
  
  // NEW: Spacing
  spacing: 'compact' | 'normal' | 'relaxed'
  
  // NEW: Images
  showImages: boolean
  imageSize: 'small' | 'medium' | 'large'
  imageShape: 'square' | 'rounded' | 'circle'
  
  // NEW: Branding
  logoPosition: 'left' | 'center' | 'right'
  showRestaurantInfo: boolean
  headerStyle: 'simple' | 'banner' | 'overlay'
}
```

#### **1.2 Visual Template Builder**

Create a live preview editor in the templates page:

**File:** `frontend/pages/dashboard/templates.vue`

Add new section:
```vue
<template>
  <!-- Existing template form -->
  
  <!-- NEW: Live Preview Section -->
  <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
    <!-- Left: Template Editor -->
    <div class="space-y-6">
      <ThemeCustomizer v-model="form.theme" />
      <LayoutCustomizer v-model="form.theme" />
      <CategoryEditor v-model="form.structure.categories" />
    </div>
    
    <!-- Right: Live Preview -->
    <div class="sticky top-4">
      <MenuPreview 
        :theme="form.theme"
        :categories="form.structure.categories"
        :restaurant="previewRestaurant"
      />
    </div>
  </div>
</template>
```

#### **1.3 Pre-built Template Library**

Create multiple industry-specific templates:

**File:** `backend/src/Menufy.Application/Features/Menus/Commands/GenerateMenu/MenuGenerationTemplates.cs`

Add templates:
```csharp
private static readonly Dictionary<string, MenuTemplate> Templates = new()
{
    ["default"] = BuildDefaultTemplate(),
    ["italian"] = BuildItalianTemplate(),
    ["fastfood"] = BuildFastFoodTemplate(),
    ["cafe"] = BuildCafeTemplate(),
    ["finedining"] = BuildFineDiningTemplate(),
    ["asian"] = BuildAsianTemplate(),
    ["mexican"] = BuildMexicanTemplate(),
};

private static MenuTemplate BuildItalianTemplate()
{
    return new MenuTemplate
    {
        Categories = new List<MenuCategoryTemplate>
        {
            new()
            {
                Name = "Antipasti",
                Translations = new() { ["ar"] = "المقبلات", ["it"] = "Antipasti" },
                Items = new()
                {
                    new() { Name = "Bruschetta", Price = 8.50m, ... },
                    new() { Name = "Caprese Salad", Price = 9.00m, ... },
                }
            },
            new()
            {
                Name = "Pasta",
                Translations = new() { ["ar"] = "المعكرونة", ["it"] = "Pasta" },
                Items = new()
                {
                    new() { Name = "Spaghetti Carbonara", Price = 14.50m, ... },
                    new() { Name = "Penne Arrabbiata", Price = 13.00m, ... },
                }
            },
            // ... more categories
        }
    };
}
```

### Phase 2: Dynamic Public Menu Styling

#### **2.1 Apply Template Theme to Public Menu**

**File:** `frontend/pages/menu/[slug].vue`

Update to apply theme dynamically:

```vue
<template>
  <div 
    :style="menuStyles"
    class="min-h-screen"
  >
    <!-- Restaurant Header with custom styling -->
    <div 
      :class="headerClasses"
      :style="headerStyles"
    >
      <RestaurantHeader 
        :restaurant="menu"
        :theme="theme"
      />
    </div>

    <!-- Menu Categories with theme -->
    <PublicMenuCategoryTree 
      :categories="menu.categories"
      :theme="theme"
      :layout="theme.layout"
    />
  </div>
</template>

<script setup lang="ts">
const theme = computed(() => menu.value?.theme || defaultTheme)

const menuStyles = computed(() => ({
  backgroundColor: theme.value.backgroundColor,
  color: theme.value.textColor,
  fontFamily: theme.value.fontFamily,
}))

const headerStyles = computed(() => ({
  backgroundColor: theme.value.primaryColor,
  color: '#ffffff',
}))
</script>
```

#### **2.2 Enhanced Menu Item Display**

**File:** `frontend/components/PublicMenuCategoryTree.vue`

Add theme-aware rendering:

```vue
<template>
  <div 
    v-for="category in categories" 
    :key="category.id"
    :class="categoryCardClasses"
    :style="categoryStyles"
  >
    <!-- Category Header -->
    <div :style="categoryHeaderStyles">
      <span v-if="category.icon" class="text-3xl mr-3">{{ category.icon }}</span>
      <h2 :style="headingStyles">{{ category.localizedName || category.name }}</h2>
    </div>

    <!-- Items Grid/List -->
    <div :class="itemsLayoutClasses">
      <MenuItem
        v-for="item in category.items"
        :key="item.id"
        :item="item"
        :theme="theme"
        :layout="theme.layout"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
const props = defineProps<{
  categories: MenuCategory[]
  theme?: TemplateTheme
}>()

const categoryStyles = computed(() => ({
  backgroundColor: props.theme?.surfaceColor || '#ffffff',
  borderRadius: getBorderRadius(props.theme?.borderRadius),
}))

const itemsLayoutClasses = computed(() => {
  const layout = props.theme?.layout || 'list'
  return {
    'grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4': layout === 'grid',
    'space-y-4': layout === 'list',
    'flex flex-wrap gap-4': layout === 'cards',
  }
})
</script>
```

### Phase 3: Advanced Customization Features

#### **3.1 Menu Item Badges & Tags**

Add visual indicators:

```vue
<template>
  <div class="menu-item">
    <!-- Item badges -->
    <div class="flex gap-2 mb-2">
      <Badge v-if="item.tags?.includes('popular')" variant="primary">
        🔥 Popular
      </Badge>
      <Badge v-if="item.tags?.includes('vegetarian')" variant="success">
        🌱 Vegetarian
      </Badge>
      <Badge v-if="item.tags?.includes('spicy')" variant="danger">
        🌶️ Spicy
      </Badge>
      <Badge v-if="item.tags?.includes('new')" variant="warning">
        ✨ New
      </Badge>
    </div>
    
    <!-- Item content -->
    <h3>{{ item.name }}</h3>
    <p>{{ item.description }}</p>
    <span class="price">{{ formatPrice(item.price) }}</span>
  </div>
</template>
```

#### **3.2 Category Icons & Emojis**

**File:** `frontend/components/CategoryIcon.vue`

```vue
<template>
  <div class="category-icon" :style="iconStyles">
    <span v-if="isEmoji" class="text-4xl">{{ icon }}</span>
    <img v-else-if="isImage" :src="icon" :alt="name" class="w-12 h-12" />
    <svg v-else class="w-12 h-12" v-html="icon"></svg>
  </div>
</template>
```

#### **3.3 Search & Filter**

Add to public menu:

```vue
<template>
  <div class="menu-controls">
    <!-- Search -->
    <input
      v-model="searchQuery"
      type="search"
      placeholder="Search menu..."
      class="search-input"
    />
    
    <!-- Filters -->
    <div class="filters">
      <button @click="filterByTag('vegetarian')">🌱 Vegetarian</button>
      <button @click="filterByTag('spicy')">🌶️ Spicy</button>
      <button @click="filterByTag('popular')">🔥 Popular</button>
      <button @click="clearFilters()">Clear</button>
    </div>
    
    <!-- Price Range -->
    <div class="price-filter">
      <label>Price Range</label>
      <input type="range" v-model="maxPrice" :max="highestPrice" />
      <span>Up to {{ formatPrice(maxPrice) }}</span>
    </div>
  </div>
</template>
```

#### **3.4 Image Gallery**

For items with multiple images:

```vue
<template>
  <div class="item-images">
    <div class="main-image">
      <img :src="currentImage" :alt="item.name" />
    </div>
    <div class="image-thumbnails">
      <img
        v-for="(img, index) in item.images"
        :key="index"
        :src="img"
        @click="currentImage = img"
        class="thumbnail"
      />
    </div>
  </div>
</template>
```

### Phase 4: Restaurant-Specific Customization

#### **4.1 Restaurant Settings Page**

**File:** `frontend/pages/dashboard/settings.vue`

```vue
<template>
  <NuxtLayout name="dashboard">
    <div class="space-y-8">
      <h1>Restaurant Settings</h1>
      
      <!-- Basic Info -->
      <Card title="Basic Information">
        <Input v-model="settings.name" label="Restaurant Name" />
        <Input v-model="settings.slug" label="URL Slug" />
        <FileUpload v-model="settings.logoUrl" label="Logo" />
        <Input v-model="settings.contactPhone" label="Phone" />
        <Input v-model="settings.contactEmail" label="Email" />
        <textarea v-model="settings.address" label="Address" />
      </Card>
      
      <!-- Menu Display Settings -->
      <Card title="Menu Display">
        <Select v-model="settings.defaultLanguage" label="Default Language">
          <option value="en">English</option>
          <option value="ar">Arabic</option>
        </Select>
        
        <Select v-model="settings.currency" label="Currency">
          <option value="USD">USD ($)</option>
          <option value="EUR">EUR (€)</option>
          <option value="SAR">SAR (ر.س)</option>
        </Select>
        
        <Toggle v-model="settings.showPrices" label="Show Prices" />
        <Toggle v-model="settings.showImages" label="Show Item Images" />
        <Toggle v-model="settings.showDescriptions" label="Show Descriptions" />
      </Card>
      
      <!-- Active Template -->
      <Card title="Active Menu Template">
        <Select v-model="settings.activeTemplateId">
          <option v-for="template in templates" :value="template.id">
            {{ template.name }}
          </option>
        </Select>
        <Button @click="applyTemplate">Apply Template</Button>
      </Card>
    </div>
  </NuxtLayout>
</template>
```

#### **4.2 Multi-Template Support**

Allow restaurants to have multiple templates for different occasions:

```typescript
// Restaurant can have multiple active templates
interface RestaurantMenuConfig {
  templates: {
    default: string      // Template ID for main menu
    breakfast?: string   // Morning menu
    lunch?: string       // Lunch specials
    dinner?: string      // Evening menu
    seasonal?: string    // Holiday/seasonal menu
  }
  activeTemplate: 'default' | 'breakfast' | 'lunch' | 'dinner' | 'seasonal'
  schedules?: {
    breakfast: { start: '06:00', end: '11:00' }
    lunch: { start: '11:00', end: '15:00' }
    dinner: { start: '17:00', end: '23:00' }
  }
}
```

---

## 🎯 Advanced Features Roadmap

### Phase 5: Analytics & Insights

```typescript
// Track menu views and popular items
interface MenuAnalytics {
  totalViews: number
  uniqueVisitors: number
  popularItems: {
    itemId: string
    viewCount: number
    averageTimeSpent: number
  }[]
  popularCategories: {
    categoryId: string
    viewCount: number
  }[]
  languagePreferences: Record<string, number>
  deviceTypes: {
    mobile: number
    tablet: number
    desktop: number
  }
}
```

### Phase 6: Customer Interaction

- **Favorites:** Allow customers to save favorite items
- **Reviews:** Customer ratings and reviews
- **Dietary Filters:** Allergen information, dietary restrictions
- **Nutritional Info:** Calories, ingredients
- **Ordering:** Direct ordering from menu (future integration)

### Phase 7: AI-Powered Features

- **Auto-translate:** AI translation for menu items
- **Image Recognition:** Upload food photos, AI generates descriptions
- **Smart Recommendations:** Suggest items based on popularity
- **Price Optimization:** AI suggests optimal pricing

---

## 📝 Implementation Checklist

### Immediate Actions

- [ ] Update template theme interface with new properties
- [ ] Create ThemeCustomizer component
- [ ] Create LayoutCustomizer component
- [ ] Add live preview to template editor
- [ ] Create pre-built template library (5-10 templates)
- [ ] Update public menu to apply theme dynamically
- [ ] Add search and filter to public menu
- [ ] Create restaurant settings page
- [ ] Add analytics tracking

### Backend Updates Needed

```csharp
// Add to Restaurant entity
public class Restaurant
{
    // ... existing properties
    
    public string? ActiveTemplateId { get; set; }
    public string? MenuDisplaySettings { get; set; } // JSON
    public string? CustomTheme { get; set; } // JSON override
}

// Add endpoint
[HttpPost("restaurants/{restaurantId}/apply-template")]
public async Task<IActionResult> ApplyTemplate(
    Guid restaurantId, 
    [FromBody] ApplyTemplateRequest request)
{
    // Apply template and regenerate menu
}

// Add endpoint
[HttpGet("restaurants/{restaurantId}/menu-analytics")]
public async Task<IActionResult> GetMenuAnalytics(Guid restaurantId)
{
    // Return analytics data
}
```

---

## 🎨 Design System Integration

Update all components to use the new design system:

```typescript
// Use primary color from design system
const theme = {
  primaryColor: '#dc2626',    // Red from design system
  accentColor: '#f59e0b',     // Amber from design system
  backgroundColor: '#fafaf9', // Neutral-50
  surfaceColor: '#ffffff',
  textColor: '#292524',       // Neutral-800
  fontFamily: 'Inter',
}
```

---

## 🚀 Quick Start Guide

### For Restaurant Owners

1. **Create a Template:**
   - Go to Dashboard → Templates
   - Click "Create Template"
   - Choose a pre-built template or start from scratch
   - Customize colors, layout, and content
   - Save template

2. **Generate Menu:**
   - Select your template
   - Click "Generate Menu"
   - Menu is created with all categories and items

3. **Generate QR Code:**
   - Go to Dashboard → QR Codes
   - Click "Generate QR Code"
   - Download and print QR code
   - Place on tables/entrance

4. **Customers Scan:**
   - Customer scans QR code
   - Opens menu in their browser
   - Can switch languages
   - View menu with your custom theme

---

## 📚 API Reference

### Generate Menu from Template

```http
POST /api/restaurants/{restaurantId}/generate-menu
Content-Type: application/json

{
  "templateId": "guid-here",
  "overwriteExisting": false,
  "targetCategories": 5,
  "targetItemsPerCategory": 10,
  "languageCodes": ["en", "ar"]
}
```

### Get Public Menu

```http
GET /api/menu/{slug}?lang=en

Response:
{
  "success": true,
  "data": {
    "restaurantId": "guid",
    "restaurantName": "My Restaurant",
    "restaurantLocalizedName": "مطعمي",
    "logoUrl": "https://...",
    "contactPhone": "+1234567890",
    "contactEmail": "info@restaurant.com",
    "address": "123 Main St",
    "language": "en",
    "theme": { ... },
    "categories": [ ... ]
  }
}
```

---

This comprehensive guide provides everything needed to understand and extend the menu generator system!

