# Phase 5: Public Menu Integration - Completion Guide

## ✅ What's Been Done

### Backend Updates ✅
1. **Updated `PublicMenuDto.cs`**:
   - Added `LayoutConfiguration` property to include per-category layouts
   - Kept legacy `Theme` and `DisplaySettings` for backward compatibility

2. **Updated `GetPublicMenuQueryHandler.cs`**:
   - Fetches active `MenuDesign` from database
   - Priority: MenuDesign > CustomTheme > ActiveTemplate > Default
   - Deserializes `LayoutConfiguration` and includes it in response
   - Backend builds successfully ✅

### What the API Now Returns

```json
{
  "restaurantId": "guid",
  "restaurantName": "My Restaurant",
  "theme": {
    "primaryColor": "#dc2626",
    "accentColor": "#f59e0b",
    ...
  },
  "layoutConfiguration": {
    "categories": [
      {
        "categoryId": "guid-1",
        "displayOrder": 1,
        "layout": "grid",
        "cardStyle": "modern",
        "columns": 3,
        "imageSize": "medium",
        "imageShape": "rounded",
        "showImages": true,
        "showPrices": true,
        "showDescriptions": true
      },
      {
        "categoryId": "guid-2",
        "displayOrder": 2,
        "layout": "list",
        ...
      }
    ],
    "globalSettings": {
      "spacing": "normal",
      "borderRadius": "medium",
      "fontFamily": "Inter",
      ...
    }
  },
  "categories": [ ... ]
}
```

---

## 🚧 What Still Needs to Be Done

### Frontend Updates Required

#### 1. Update `PublicMenuCategoryTree.vue`

**Current State**: Renders all categories the same way (list style)

**Needed Changes**:
- Accept `layoutConfiguration` prop
- For each category, look up its layout settings by `categoryId`
- Render dynamically based on layout type:

**Pseudocode**:
```typescript
// Get layout for this category
const categoryLayout = layoutConfiguration?.categories.find(c => c.categoryId === category.id)
const layout = categoryLayout?.layout || 'list' // default

// Render based on layout
if (layout === 'grid') {
  // Use CSS Grid with columns
  <div class="grid" :style="{gridTemplateColumns: `repeat(${columns}, 1fr)`}">
} else if (layout === 'cards') {
  // Card-based layout
  <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
} else {
  // List layout (current default)
  <div class="space-y-4">
}
```

**Key Props to Use**:
- `layout`: "list" | "grid" | "cards"
- `card Style`: "modern" | "classic" | "minimal"
- `columns`: number (for grid)
- `imageSize`: "small" | "medium" | "large"
- `imageShape`: "square" | "rounded" | "circle"
- `showImages`, `showPrices`, `showDescriptions`: boolean

**Implementation Strategy**:
```vue
<template>
  <div v-for="category in categories" :key="category.id">
    <!-- Get layout for this category -->
    <component 
      :is="getCategoryComponent(category.id)"
      :category="category"
      :layout-settings="getCategoryLayout(category.id)"
      :theme="theme"
      ...
    />
  </div>
</template>

<script setup>
const props = defineProps<{
  categories: MenuCategory[]
  layoutConfiguration?: LayoutConfigurationDto | null
  theme?: TemplateTheme | null
  ...
}>()

const getCategoryLayout = (categoryId: string) => {
  return props.layoutConfiguration?.categories.find(c => c.categoryId === categoryId)
}

// Or alternatively, render inline:
const getContainerClass = (categoryId: string) => {
  const layout = getCategoryLayout(categoryId)?.layout || 'list'
  
  if (layout === 'grid') {
    const cols = layout.columns || 3
    return `grid grid-cols-1 md:grid-cols-${cols} gap-4`
  } else if (layout === 'cards') {
    return 'grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6'
  }
  return 'space-y-4' // list
}
</script>
```

#### 2. Update `menu/[slug].vue`

**Current State**: Passes `settings` and `displaySettings` to component

**Needed Changes**:
- Pass `layoutConfiguration` to `PublicMenuCategoryTree`

```vue
<PublicMenuCategoryTree
  :categories="filteredCategories"
  :settings="settings"
  :displaySettings="displaySettings"
  :layout-configuration="menu.layoutConfiguration"  <!-- ADD THIS -->
  :currency="menu.currency"
  @item-click="openItemDetail"
/>
```

---

## 📝 Detailed Implementation Steps

### Step 1: Update Props in PublicMenuCategoryTree.vue

```typescript
const props = withDefaults(defineProps<{
  categories: MenuCategory[]
  settings?: any
  displaySettings?: any
  currency?: string
  layoutConfiguration?: LayoutConfigurationDto | null  // NEW
}>(), {
  settings: null,
  displaySettings: null,
  currency: 'USD',
  layoutConfiguration: null  // NEW
})
```

### Step 2: Create Helper Functions

```typescript
// Get layout settings for a category
const getCategoryLayout = (categoryId: string) => {
  if (!props.layoutConfiguration) return null
  return props.layoutConfiguration.categories.find(c => c.categoryId === categoryId)
}

// Get container classes based on layout
const getContainerClass = (categoryId: string) => {
  const layout = getCategoryLayout(categoryId)
  if (!layout) return 'space-y-4' // default list
  
  switch (layout.layout) {
    case 'grid':
      return `grid gap-4 grid-cols-1 md:grid-cols-${layout.columns || 3}`
    case 'cards':
      return 'grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6'
    default:
      return 'space-y-4'
  }
}

// Get item card classes
const getItemCardClass = (categoryId: string) => {
  const layout = getCategoryLayout(categoryId)
  if (!layout) return 'p-4 rounded-lg border'
  
  const baseClasses = ['p-4', 'rounded-lg', 'transition-all', 'cursor-pointer']
  
  switch (layout.cardStyle) {
    case 'modern':
      return [...baseClasses, 'bg-gradient-to-br', 'from-white', 'to-neutral-50', 'shadow-lg', 'hover:shadow-xl'].join(' ')
    case 'classic':
      return [...baseClasses, 'bg-white', 'border-2', 'border-neutral-300', 'hover:border-primary-500'].join(' ')
    case 'minimal':
      return [...baseClasses, 'bg-neutral-50', 'hover:bg-neutral-100'].join(' ')
    default:
      return baseClasses.join(' ')
  }
}

// Get image size classes
const getImageClass = (categoryId: string) => {
  const layout = getCategoryLayout(categoryId)
  if (!layout || !layout.showImages) return ''
  
  const size = layout.imageSize || 'medium'
  const shape = layout.imageShape || 'rounded'
  
  let classes = []
  
  // Size
  if (size === 'small') classes.push('w-16 h-16')
  else if (size === 'large') classes.push('w-32 h-32')
  else classes.push('w-24 h-24')
  
  // Shape
  if (shape === 'circle') classes.push('rounded-full')
  else if (shape === 'rounded') classes.push('rounded-lg')
  
  return classes.join(' ')
}
```

### Step 3: Update Template

```vue
<template>
  <div
    v-for="category in categories"
    :key="category.id"
    class="mb-8 rounded-3xl shadow-xl overflow-hidden"
    :style="{ backgroundColor: settings?.surfaceColor || '#ffffff' }"
  >
    <!-- Category Header (unchanged) -->
    <div v-if="shouldShowCategory(category.id)" class="px-8 py-6">
      <h2>{{ category.localizedName || category.name }}</h2>
    </div>

    <!-- Items Container - DYNAMIC BASED ON LAYOUT -->
    <div class="p-6" :class="getContainerClass(category.id)">
      <div
        v-for="item in category.items.filter(i => i.isAvailable)"
        :key="item.id"
        :class="getItemCardClass(category.id)"
        @click="$emit('item-click', item, category)"
      >
        <!-- Image (conditional based on layout) -->
        <div v-if="shouldShowImage(category.id)" :class="getImageClass(category.id)">
          <img :src="item.imageUrl" :alt="item.name" class="w-full h-full object-cover" />
        </div>

        <!-- Content -->
        <div>
          <h3>{{ item.localizedName || item.name }}</h3>
          <p v-if="shouldShowDescription(category.id)">
            {{ item.localizedDescription || item.description }}
          </p>
          <span v-if="shouldShowPrice(category.id)">
            {{ formatPrice(item.price) }}
          </span>
        </div>
      </div>
    </div>

    <!-- Children (recursive) -->
    <PublicMenuCategoryTree
      v-if="category.children?.length"
      :categories="category.children"
      :layout-configuration="layoutConfiguration"
      :settings="settings"
      :displaySettings="displaySettings"
      :currency="currency"
      @item-click="$emit('item-click', $event)"
    />
  </div>
</template>
```

### Step 4: Add Visibility Helpers

```typescript
const shouldShowImage = (categoryId: string) => {
  const layout = getCategoryLayout(categoryId)
  if (layout) return layout.showImages
  return props.displaySettings?.showImages ?? true
}

const shouldShowPrice = (categoryId: string) => {
  const layout = getCategoryLayout(categoryId)
  if (layout) return layout.showPrices
  return props.displaySettings?.showPrices ?? true
}

const shouldShowDescription = (categoryId: string) => {
  const layout = getCategoryLayout(categoryId)
  if (layout) return layout.showDescriptions
  return props.displaySettings?.showDescriptions ?? true
}

const shouldShowCategory = (categoryId: string) => {
  return props.displaySettings?.showCategories !== false
}
```

---

## 🎯 Expected Result

After implementation:
1. Designer saves layout preferences per category
2. Backend includes `layoutConfiguration` in public menu response
3. Frontend renders each category according to its saved layout:
   - **Pizza category**: Grid with 3 columns, modern cards, large images
   - **Drinks category**: List layout, classic style, small images
   - **Desserts category**: Cards layout, minimal style, no images

---

## 🧪 Testing Checklist

After implementation, test:
- [ ] Categories with different layouts render correctly
- [ ] Grid layout respects column settings
- [ ] Card styles apply correctly (modern/classic/minimal)
- [ ] Image size and shape settings work
- [ ] Show/hide toggles per category work
- [ ] Fallback to default when no design saved
- [ ] Responsive on mobile
- [ ] Nested categories work correctly

---

## ⚡ Quick Win Alternative

If full implementation is complex, you can start simple:

**Minimal Version**:
```vue
<!-- Just check if layoutConfiguration exists and use it for container class -->
<div :class="getSimpleContainerClass(category.id)">
  <!-- Keep existing item rendering -->
</div>

<script>
const getSimpleContainerClass = (categoryId: string) => {
  const layout = props.layoutConfiguration?.categories.find(c => c.categoryId === categoryId)
  
  if (layout?.layout === 'grid') {
    return 'grid grid-cols-2 md:grid-cols-3 gap-4'
  }
  
  return 'space-y-4' // default list
}
</script>
```

This gives you grid/list switching without all the complexity, then iterate from there.

---

## 📚 Reference

### Type Definitions
```typescript
interface LayoutConfigurationDto {
  categories: CategoryLayoutDto[]
  globalSettings: GlobalLayoutSettingsDto
}

interface CategoryLayoutDto {
  categoryId: string
  displayOrder: number
  layout: 'list' | 'grid' | 'cards'
  cardStyle: 'modern' | 'classic' | 'minimal'
  columns?: number
  spacing?: 'compact' | 'normal' | 'relaxed'
  borderRadius?: 'none' | 'small' | 'medium' | 'large'
  imageSize?: 'small' | 'medium' | 'large'
  imageShape?: 'square' | 'rounded' | 'circle'
  showImages: boolean
  showPrices: boolean
  showDescriptions: boolean
}
```

---

## 🎉 When Complete

You'll have a fully functional visual menu designer where:
1. Restaurant owners design menus in the designer
2. Designs save to database with per-category layouts
3. Public menus render exactly as designed
4. Each category can have its own unique layout and style!

**This is the final piece to make the entire redesign work end-to-end.**

