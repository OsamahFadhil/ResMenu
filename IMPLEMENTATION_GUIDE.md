# Implementation Guide - Unified Designer System 🚀

## Overview
This guide provides detailed steps to transform the current system into the new unified designer architecture.

---

## 📅 Timeline: 2 Weeks

### **Week 1**: Core Designer + Live Data
### **Week 2**: Settings Simplification + Migration

---

## Phase 1: Core Designer Updates (Days 1-3)

### **Day 1: Left Panel - Data Sources**

#### **1.1 Create Data Source Panel Component**
```bash
frontend/components/designer/DataSourcePanel.vue
```

**Features**:
- Collapsible sections: Menu Data, Restaurant Info, Design Elements
- Display categories from restaurant store
- Display restaurant info (name, logo, contact)
- Search/filter categories
- Drag handles on each item

**Code Structure**:
```vue
<template>
  <div class="w-80 bg-neutral-800 overflow-y-auto">
    <!-- Search -->
    <input type="text" v-model="search" placeholder="Search..." />

    <!-- Menu Data Section -->
    <div class="section">
      <h3>📊 MENU DATA</h3>

      <!-- Categories -->
      <div
        v-for="category in filteredCategories"
        :key="category.id"
        draggable="true"
        @dragstart="onDragCategory(category)"
        class="category-item"
      >
        🍕 {{ category.name }} ({{ category.items.length }})
      </div>
    </div>

    <!-- Restaurant Info Section -->
    <div class="section">
      <h3>ℹ️ RESTAURANT INFO</h3>

      <div draggable="true" @dragstart="onDragInfo('name')">
        🏪 Restaurant Name
      </div>
      <div draggable="true" @dragstart="onDragInfo('logo')">
        🖼️ Logo
      </div>
      <div draggable="true" @dragstart="onDragInfo('contact')">
        📞 Contact
      </div>
    </div>

    <!-- Design Elements Section -->
    <div class="section">
      <h3>🎨 DESIGN ELEMENTS</h3>
      <div draggable="true" @dragstart="onDragElement('text')">✏️ Text</div>
      <div draggable="true" @dragstart="onDragElement('image')">🖼️ Image</div>
      <div draggable="true" @dragstart="onDragElement('shape')">⬛ Shape</div>
    </div>
  </div>
</template>

<script setup>
const restaurantStore = useRestaurantStore()
const search = ref('')

const filteredCategories = computed(() => {
  return restaurantStore.categories.filter(c =>
    c.name.toLowerCase().includes(search.value.toLowerCase())
  )
})

const onDragCategory = (category) => {
  // Set drag data
  event.dataTransfer.setData('type', 'category')
  event.dataTransfer.setData('categoryId', category.id)
}

const onDragInfo = (type) => {
  event.dataTransfer.setData('type', type)
}

const onDragElement = (type) => {
  event.dataTransfer.setData('type', type)
}
</script>
```

#### **1.2 Update Designer Canvas for Drop**
```vue
<!-- frontend/components/designer/DesignerCanvas.vue -->

<div
  @drop="onDrop"
  @dragover.prevent
  class="canvas"
>
  <!-- Render elements -->
</div>

<script setup>
const onDrop = (event) => {
  event.preventDefault()

  const type = event.dataTransfer.getData('type')
  const rect = event.currentTarget.getBoundingClientRect()
  const x = event.clientX - rect.left
  const y = event.clientY - rect.top

  if (type === 'category') {
    const categoryId = event.dataTransfer.getData('categoryId')
    addCategoryElement(categoryId, x, y)
  } else if (type === 'name') {
    addRestaurantNameElement(x, y)
  } else if (type === 'logo') {
    addLogoElement(x, y)
  }
  // ... handle other types
}
</script>
```

---

### **Day 2: Category Element Type**

#### **2.1 Update MenuDesignElement Interface**
```typescript
// frontend/stores/menuDesigner.ts

export interface MenuDesignElement {
  // ... existing properties

  // NEW: Category element properties
  categoryId?: string          // Links to DB category
  layoutStyle?: 'list' | 'grid' | 'cards' | 'horizontal'
  gridColumns?: number
  itemGap?: number
  showImages?: boolean
  showDescriptions?: boolean
  showPrices?: boolean
  showTags?: boolean
}
```

#### **2.2 Create Category Element Component**
```vue
<!-- frontend/components/designer/elements/CategoryElement.vue -->

<template>
  <div :style="containerStyle" class="category-element">
    <h2 class="category-title">{{ category.name }}</h2>

    <!-- List Layout -->
    <div v-if="element.layoutStyle === 'list'" class="layout-list">
      <div
        v-for="item in category.items"
        :key="item.id"
        class="item-row"
        :style="itemStyle"
      >
        <img v-if="element.showImages && item.imageUrl" :src="item.imageUrl" />
        <div class="item-info">
          <span class="item-name">{{ item.name }}</span>
          <span v-if="element.showDescriptions" class="item-desc">{{ item.description }}</span>
        </div>
        <span v-if="element.showPrices" class="item-price">${{ item.price }}</span>
      </div>
    </div>

    <!-- Grid Layout -->
    <div
      v-else-if="element.layoutStyle === 'grid'"
      class="layout-grid"
      :style="gridStyle"
    >
      <div
        v-for="item in category.items"
        :key="item.id"
        class="item-card"
        :style="cardStyle"
      >
        <img v-if="element.showImages && item.imageUrl" :src="item.imageUrl" />
        <span class="item-name">{{ item.name }}</span>
        <span v-if="element.showPrices" class="item-price">${{ item.price }}</span>
      </div>
    </div>

    <!-- Cards Layout -->
    <div v-else-if="element.layoutStyle === 'cards'" class="layout-cards">
      <!-- Similar to grid but full width cards -->
    </div>
  </div>
</template>

<script setup>
const props = defineProps(['element'])
const restaurantStore = useRestaurantStore()

// Fetch category and items from DB
const category = computed(() => {
  return restaurantStore.categories.find(c => c.id === props.element.categoryId)
})

const gridStyle = computed(() => ({
  display: 'grid',
  gridTemplateColumns: `repeat(${props.element.gridColumns || 2}, 1fr)`,
  gap: `${props.element.itemGap || 20}px`
}))

const itemStyle = computed(() => ({
  backgroundColor: props.element.styling?.backgroundColor || '#fff',
  color: props.element.styling?.textColor || '#333',
  fontSize: `${props.element.styling?.fontSize || 16}px`,
  padding: `${props.element.styling?.padding || 20}px`,
  borderRadius: `${props.element.styling?.borderRadius || 0}px`
}))
</script>
```

#### **2.3 Add Category to Element Renderer**
```vue
<!-- frontend/components/designer/DesignerElement.vue -->

<component
  :is="getElementComponent(element.type)"
  :element="element"
/>

<script setup>
const getElementComponent = (type) => {
  switch (type) {
    case 'text': return TextElement
    case 'image': return ImageElement
    case 'shape': return ShapeElement
    case 'category': return CategoryElement  // NEW
    case 'restaurantName': return RestaurantNameElement  // NEW
    case 'logo': return LogoElement  // NEW
    default: return TextElement
  }
}
</script>
```

---

### **Day 3: Properties Panel Updates**

#### **3.1 Category Properties Component**
```vue
<!-- frontend/components/designer/properties/CategoryProperties.vue -->

<template>
  <div class="properties-panel">
    <h3>Category Element</h3>

    <!-- Category Selector -->
    <div class="property-group">
      <label>Category:</label>
      <select v-model="localElement.categoryId" @change="updateElement">
        <option v-for="cat in categories" :key="cat.id" :value="cat.id">
          {{ cat.name }} ({{ cat.items.length }} items)
        </option>
      </select>
    </div>

    <!-- Layout Style -->
    <div class="property-group">
      <label>Layout Style:</label>
      <div class="radio-group">
        <label>
          <input type="radio" v-model="localElement.layoutStyle" value="list" @change="updateElement" />
          List
        </label>
        <label>
          <input type="radio" v-model="localElement.layoutStyle" value="grid" @change="updateElement" />
          Grid
        </label>
        <label>
          <input type="radio" v-model="localElement.layoutStyle" value="cards" @change="updateElement" />
          Cards
        </label>
      </div>
    </div>

    <!-- Grid Columns (if grid layout) -->
    <div v-if="localElement.layoutStyle === 'grid'" class="property-group">
      <label>Grid Columns:</label>
      <input
        type="range"
        v-model="localElement.gridColumns"
        min="1"
        max="4"
        @input="updateElement"
      />
      <span>{{ localElement.gridColumns }}</span>
    </div>

    <!-- Item Gap -->
    <div class="property-group">
      <label>Item Gap:</label>
      <input
        type="range"
        v-model="localElement.itemGap"
        min="0"
        max="50"
        @input="updateElement"
      />
      <span>{{ localElement.itemGap }}px</span>
    </div>

    <!-- Display Options -->
    <div class="property-group">
      <label>Display Options:</label>
      <label>
        <input type="checkbox" v-model="localElement.showImages" @change="updateElement" />
        Show Images
      </label>
      <label>
        <input type="checkbox" v-model="localElement.showDescriptions" @change="updateElement" />
        Show Descriptions
      </label>
      <label>
        <input type="checkbox" v-model="localElement.showPrices" @change="updateElement" />
        Show Prices
      </label>
    </div>

    <!-- Styling -->
    <div class="property-group">
      <h4>Styling</h4>

      <label>Background Color:</label>
      <input
        type="color"
        v-model="localElement.styling.backgroundColor"
        @input="updateElement"
      />

      <label>Text Color:</label>
      <input
        type="color"
        v-model="localElement.styling.textColor"
        @input="updateElement"
      />

      <label>Font Size:</label>
      <input
        type="range"
        v-model="localElement.styling.fontSize"
        min="12"
        max="32"
        @input="updateElement"
      />
      <span>{{ localElement.styling.fontSize }}px</span>
    </div>

    <!-- Actions -->
    <button @click="duplicateElement">Duplicate</button>
    <button @click="deleteElement">Delete</button>
  </div>
</template>
```

---

## Phase 2: Live Data Integration (Days 4-5)

### **Day 4: Backend - MenuDesigns Table & API**

#### **4.1 Create Migration**
```sql
-- backend/src/Menufy.Infrastructure/Migrations/AddMenuDesigns.sql

CREATE TABLE MenuDesigns (
    Id VARCHAR(36) PRIMARY KEY,
    RestaurantId VARCHAR(36) NOT NULL,
    Name VARCHAR(200) NOT NULL,
    IsActive BOOLEAN DEFAULT FALSE,
    DesignConfig JSON NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (RestaurantId) REFERENCES Restaurants(Id) ON DELETE CASCADE,
    INDEX idx_restaurant_active (RestaurantId, IsActive)
);
```

#### **4.2 Create MenuDesign Entity**
```csharp
// backend/src/Menufy.Domain/Entities/MenuDesign.cs

public class MenuDesign
{
    public string Id { get; set; }
    public string RestaurantId { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public string DesignConfig { get; set; }  // JSON
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Restaurant Restaurant { get; set; }
}
```

#### **4.3 Create MenuDesignsController**
```csharp
// backend/src/Menufy.API/Controllers/MenuDesignsController.cs

[ApiController]
[Route("api/restaurants/{restaurantId}/menu-designs")]
public class MenuDesignsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDesigns(string restaurantId)
    {
        // Return all designs for restaurant
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDesign(string restaurantId, string id)
    {
        // Return specific design
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveDesign(string restaurantId)
    {
        // Return active design
    }

    [HttpPost]
    public async Task<IActionResult> CreateDesign(
        string restaurantId,
        [FromBody] CreateDesignRequest request
    )
    {
        // Create new design
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDesign(
        string restaurantId,
        string id,
        [FromBody] UpdateDesignRequest request
    )
    {
        // Update design
    }

    [HttpPost("{id}/publish")]
    public async Task<IActionResult> PublishDesign(string restaurantId, string id)
    {
        // Set design as active
        // Deactivate other designs
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDesign(string restaurantId, string id)
    {
        // Delete design
    }
}
```

---

### **Day 5: Frontend - Save/Load/Publish**

#### **5.1 Create useMenuDesign Composable**
```typescript
// frontend/composables/useMenuDesign.ts

export const useMenuDesign = () => {
  const api = useApi()

  const saveDesign = async (restaurantId: string, design: MenuDesignProject) => {
    return await api.post(`/restaurants/${restaurantId}/menu-designs`, {
      name: design.name,
      designConfig: JSON.stringify({
        width: design.width,
        height: design.height,
        backgroundColor: design.backgroundColor,
        backgroundImage: design.backgroundImage,
        elements: design.elements
      })
    })
  }

  const loadDesigns = async (restaurantId: string) => {
    return await api.get(`/restaurants/${restaurantId}/menu-designs`)
  }

  const getActiveDesign = async (restaurantId: string) => {
    return await api.get(`/restaurants/${restaurantId}/menu-designs/active`)
  }

  const publishDesign = async (restaurantId: string, designId: string) => {
    return await api.post(`/restaurants/${restaurantId}/menu-designs/${designId}/publish`)
  }

  return {
    saveDesign,
    loadDesigns,
    getActiveDesign,
    publishDesign
  }
}
```

#### **5.2 Update Designer to Save/Publish**
```vue
<!-- frontend/pages/dashboard/designer.vue -->

<button @click="saveDesignToDB">Save</button>
<button @click="publishDesignToDB">Publish</button>

<script setup>
const { saveDesign, publishDesign } = useMenuDesign()
const authStore = useAuthStore()
const toast = useToast()

const saveDesignToDB = async () => {
  if (!store.currentProject) return

  try {
    const result = await saveDesign(
      authStore.restaurantId,
      store.currentProject
    )

    toast.success('Design saved successfully!')

    // Update current project with saved ID
    if (result.data.id) {
      store.currentProject.id = result.data.id
    }
  } catch (error) {
    toast.error('Failed to save design')
  }
}

const publishDesignToDB = async () => {
  if (!store.currentProject?.id) {
    toast.error('Please save the design first')
    return
  }

  try {
    await publishDesign(authStore.restaurantId, store.currentProject.id)
    toast.success('Design published! Your menu is now live.')
  } catch (error) {
    toast.error('Failed to publish design')
  }
}
</script>
```

---

## Phase 3: Public Menu Integration (Days 6-7)

### **Day 6: Update Public Menu to Use Design**

#### **6.1 Create Menu Renderer Component**
```vue
<!-- frontend/components/menu/MenuRenderer.vue -->

<template>
  <div
    class="menu-container"
    :style="{
      width: `${design.width}px`,
      backgroundColor: design.backgroundColor,
      backgroundImage: design.backgroundImage ? `url(${design.backgroundImage})` : 'none'
    }"
  >
    <component
      v-for="element in sortedElements"
      :key="element.id"
      :is="getElementComponent(element.type)"
      :element="element"
      :restaurant="restaurant"
      :categories="categories"
    />
  </div>
</template>

<script setup>
const props = defineProps(['design', 'restaurant', 'categories'])

const sortedElements = computed(() => {
  return [...props.design.elements].sort((a, b) => a.zIndex - b.zIndex)
})

const getElementComponent = (type) => {
  switch (type) {
    case 'category': return CategoryRender
    case 'restaurantName': return RestaurantNameRender
    case 'logo': return LogoRender
    case 'text': return TextRender
    case 'image': return ImageRender
    case 'shape': return ShapeRender
    default: return TextRender
  }
}
</script>
```

#### **6.2 Update Public Menu Page**
```vue
<!-- frontend/pages/[slug].vue -->

<template>
  <div>
    <!-- If has active design -->
    <MenuRenderer
      v-if="activeDesign"
      :design="activeDesign"
      :restaurant="restaurant"
      :categories="categories"
    />

    <!-- Fallback to default layout -->
    <DefaultMenuLayout
      v-else
      :restaurant="restaurant"
      :categories="categories"
    />
  </div>
</template>

<script setup>
const route = useRoute()
const { getActiveDesign } = useMenuDesign()

// Fetch active design
const { data: activeDesign } = await useAsyncData(
  `design-${route.params.slug}`,
  () => getActiveDesign(restaurant.value.id)
)

// Fetch restaurant and categories (existing code)
// ...
</script>
```

---

## Phase 4: Settings Simplification (Days 8-9)

### **Day 8: Update Restaurant Info**

#### **8.1 Create RestaurantInfo Interface**
```typescript
// frontend/types/restaurant.ts

export interface RestaurantInfo {
  id: string
  name: string
  localizedNames: Record<string, string>
  logo: string | null
  contactPhone: string | null
  email: string | null
  address: string | null
  businessHours: Record<string, string>
  currency: string
  defaultLanguage: string
  displaySettings: {
    showPrices: boolean
    showImages: boolean
    showDescriptions: boolean
    enableSearch: boolean
    enableFilters: boolean
  }
}
```

#### **8.2 Simplify Settings Page**
```vue
<!-- frontend/pages/dashboard/settings.vue -->

<template>
  <div class="settings-page">
    <h1>Restaurant Settings</h1>

    <!-- Basic Information -->
    <Card>
      <h2>Basic Information</h2>

      <FormField label="Restaurant Name">
        <input v-model="restaurantInfo.name" />
      </FormField>

      <FormField label="Arabic Name">
        <input v-model="restaurantInfo.localizedNames.ar" />
      </FormField>

      <!-- ... other fields -->
    </Card>

    <!-- Logo -->
    <Card>
      <h2>Logo</h2>
      <ImageUpload v-model="restaurantInfo.logo" />
    </Card>

    <!-- Contact Information -->
    <Card>
      <h2>Contact Information</h2>

      <FormField label="Phone">
        <input v-model="restaurantInfo.contactPhone" />
      </FormField>

      <FormField label="Email">
        <input v-model="restaurantInfo.email" />
      </FormField>

      <FormField label="Address">
        <textarea v-model="restaurantInfo.address" />
      </FormField>
    </Card>

    <!-- Display Options -->
    <Card>
      <h2>Display Options</h2>

      <Checkbox v-model="restaurantInfo.displaySettings.showPrices">
        Show Prices on Menu
      </Checkbox>

      <Checkbox v-model="restaurantInfo.displaySettings.showImages">
        Show Images
      </Checkbox>

      <!-- ... other options -->
    </Card>

    <button @click="saveRestaurantInfo">Save Settings</button>
  </div>
</template>

<!-- REMOVED: Template selection, theme customization -->
```

---

### **Day 9: Remove Template System**

#### **9.1 Delete Files**
```bash
# Remove template page
rm frontend/pages/dashboard/templates.vue

# Keep template store for now (migration)
# Will remove after migration complete
```

#### **9.2 Remove from Navigation**
```vue
<!-- frontend/layouts/dashboard.vue -->

<!-- REMOVE this navigation item -->
<NuxtLink to="/dashboard/templates">Templates</NuxtLink>
```

#### **9.3 Add Design Presets to Designer**
```typescript
// frontend/stores/menuDesigner.ts

const designPresets = [
  {
    id: 'classic-list',
    name: 'Classic List',
    description: 'Traditional list layout',
    elements: [
      { type: 'restaurantName', x: 50, y: 20, ... },
      { type: 'logo', x: 200, y: 20, ... },
      { type: 'category', categoryId: null, layoutStyle: 'list', ... }
    ]
  },
  {
    id: 'modern-grid',
    name: 'Modern Grid',
    description: 'Contemporary grid layout',
    elements: [...]
  }
  // ... more presets
]

// Method to load preset
const loadPreset = (presetId: string) => {
  const preset = designPresets.find(p => p.id === presetId)
  if (!preset) return

  // Create new project with preset elements
  // Replace categoryId: null with actual category IDs
  const elements = preset.elements.map((el, index) => {
    if (el.type === 'category' && !el.categoryId) {
      // Assign to first, second, third category...
      el.categoryId = restaurantStore.categories[index]?.id
    }
    return el
  })

  createProject('New Design', restaurantId, { elements })
}
```

---

## Phase 5: Migration & Testing (Days 10-14)

### **Day 10-11: Data Migration**

#### **10.1 Migration Script**
```typescript
// scripts/migrate-templates-to-designs.ts

async function migrateTemplateToDesign(template: MenuTemplate) {
  // Convert template structure to design elements
  const elements: MenuDesignElement[] = []
  let yPosition = 150

  // Add restaurant name element
  elements.push({
    type: 'restaurantName',
    x: 50,
    y: 20,
    width: 300,
    height: 60,
    styling: { fontSize: 48, color: '#000' }
  })

  // Add logo element
  elements.push({
    type: 'logo',
    x: 200,
    y: 20,
    width: 100,
    height: 100
  })

  // Convert each template category to category element
  template.structure.categories.forEach((category, index) => {
    elements.push({
      type: 'category',
      categoryId: category.id,  // Map to actual DB category
      x: 50,
      y: yPosition,
      width: 980,
      height: 300,
      layoutStyle: 'list',
      showImages: true,
      showDescriptions: true,
      showPrices: true,
      styling: {
        backgroundColor: template.theme?.surfaceColor || '#fff',
        textColor: template.theme?.textColor || '#333'
      }
    })

    yPosition += 350
  })

  // Create menu design
  const design = {
    restaurantId: template.restaurantId,
    name: `Migrated: ${template.name}`,
    isActive: false,  // Don't auto-activate
    designConfig: {
      width: 1080,
      height: yPosition + 100,
      backgroundColor: template.theme?.backgroundColor || '#fff',
      elements
    }
  }

  await saveDesign(design)
}

// Run migration for all templates
const templates = await getAllTemplates()
for (const template of templates) {
  await migrateTemplateToDesign(template)
}
```

---

### **Day 12-13: Testing**

#### **Testing Checklist**

**Designer Testing**:
- [ ] Drag category from left panel to canvas
- [ ] Category displays live items from DB
- [ ] Switch between layout styles (list, grid, cards)
- [ ] Adjust styling (colors, fonts, spacing)
- [ ] Drag restaurant name, logo, contact
- [ ] Save design to database
- [ ] Load saved designs
- [ ] Publish design
- [ ] Multiple designs per restaurant

**Public Menu Testing**:
- [ ] Visit public menu URL
- [ ] Menu uses active design layout
- [ ] Items display correctly in chosen layout
- [ ] Add new item in dashboard
- [ ] Verify item appears on public menu automatically
- [ ] Change item price
- [ ] Verify price updates on public menu
- [ ] No republish needed

**Settings Testing**:
- [ ] Update restaurant name
- [ ] Verify name updates in designer
- [ ] Verify name updates on public menu
- [ ] Upload logo
- [ ] Verify logo appears in designer
- [ ] Verify logo appears on public menu
- [ ] Update contact info
- [ ] Verify contact updates everywhere

**Edge Cases**:
- [ ] Restaurant with no categories
- [ ] Category with no items
- [ ] Design with deleted category
- [ ] Design without active status
- [ ] Multiple browser tabs

---

### **Day 14: Polish & Documentation**

#### **14.1 Add Loading States**
- Designer loading skeleton
- Public menu loading state
- Save/publish progress indicators

#### **14.2 Add Error Handling**
- Category not found
- Design config invalid
- Network errors
- Permission errors

#### **14.3 Add User Guidance**
- Empty state in left panel (no categories)
- Tutorial/onboarding for new users
- Tooltips on properties
- Keyboard shortcuts help

#### **14.4 Update Documentation**
```markdown
# How to Design Your Menu

1. Go to Dashboard → Menu Designer
2. Drag categories from left panel to canvas
3. Customize layout and styling
4. Click "Publish" to make it live
5. Menu updates automatically when you add/edit items
```

---

## Rollback Plan

If issues arise:

1. **Keep old system running** in parallel initially
2. **Feature flag** to switch between old/new
3. **Database backup** before migration
4. **Quick revert**: Re-enable templates page, disable designer publish

```typescript
// Feature flag
const useNewDesigner = process.env.NUXT_PUBLIC_USE_NEW_DESIGNER === 'true'
```

---

## Post-Implementation

### **Cleanup (Week 3+)**
- [ ] Remove template store completely
- [ ] Remove template-related API endpoints
- [ ] Remove template migration scripts
- [ ] Archive old documentation
- [ ] Monitor performance
- [ ] Gather user feedback

### **Future Enhancements**
- [ ] More layout styles
- [ ] Animation options
- [ ] Mobile-specific designs
- [ ] A/B testing designs
- [ ] Analytics on design performance
- [ ] Community design marketplace

---

## Success Metrics

Track these metrics:
- Time to create first menu design
- Number of design iterations
- Time from menu update to public visibility
- User satisfaction (surveys)
- Support tickets related to menu design
- Design publish rate

---

**This implementation guide provides step-by-step instructions for the complete system transformation.**
