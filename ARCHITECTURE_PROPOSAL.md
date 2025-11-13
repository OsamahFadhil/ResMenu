# Menu System Architecture - Analysis & Proposal 🏗️

## 📊 Current System Analysis

### 1. **Current Template System** (`/dashboard/templates`)
**Purpose**: Create reusable menu structure templates with categories and items

**Features**:
- Create/Edit/Delete templates
- Define categories and menu items
- Set template theme (colors, fonts, layout)
- Generate menu from template
- Apply template to restaurant

**Data Structure**:
```typescript
MenuTemplate {
  id, name, description
  structure: {
    categories: [
      { name, items: [...] }
    ]
  }
  theme: {
    colors, fonts, layout
  }
}
```

**Issues**:
- ❌ Separates data (templates) from design (designer)
- ❌ Template changes don't auto-update live menu
- ❌ No visual preview of template
- ❌ Manual "Apply" step required

---

### 2. **Current Settings Page** (`/dashboard/settings`)
**Purpose**: Configure restaurant settings and select active template

**Features**:
- Select active template
- Apply template to menu
- Customize theme (colors, fonts, layout)
- Display settings

**Data Structure**:
```typescript
RestaurantSettings {
  activeTemplateId
  customTheme: {
    colors, fonts, layout
  }
  displaySettings
  currency, defaultLanguage
}
```

**Issues**:
- ❌ Mixes restaurant info with design settings
- ❌ Theme customization separate from designer
- ❌ Duplicate theme settings (template theme vs custom theme)
- ❌ No visual design preview

---

### 3. **Current Designer** (`/dashboard/designer`)
**Purpose**: Visual drag-and-drop menu design tool

**Features**:
- Drag-and-drop elements (text, images, shapes, menu items)
- Resize, rotate, reposition elements
- Layer management
- Export as PNG/PDF/SVG
- Template library (5 pre-designed templates)

**Data Structure**:
```typescript
MenuDesignProject {
  id, name, restaurantId
  elements: [
    { type, x, y, width, height, rotation, ...props }
  ]
  backgroundColor, backgroundImage
}
```

**Issues**:
- ❌ Designer is **completely separate** from actual menu data
- ❌ Designs are static - no live data from categories/items
- ❌ Saved designs don't apply to public menu
- ❌ Manual element creation (not connected to real menu)
- ❌ Export-only (doesn't integrate with menu system)

---

## 🎯 Proposed Architecture: **Unified Designer System**

### Core Concept
**Designer becomes the ONLY place to design menu layout** - it pulls real categories/items and applies design directly to public menu.

---

## 🏗️ New System Architecture

### **1. Simplified Settings** (`/dashboard/settings`)
**NEW Purpose**: Restaurant information ONLY

**Features**:
✅ Restaurant name & localized names
✅ Logo upload
✅ Contact information
✅ Business hours
✅ Currency & languages
✅ Basic display options (show prices, images, etc.)

**Remove**:
❌ Template selection
❌ Theme customization
❌ Layout settings

**New Data Structure**:
```typescript
RestaurantInfo {
  name: string
  localizedNames: Record<string, string>
  logo: string
  contactPhone: string
  address: string
  email: string
  businessHours: {...}
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

---

### **2. Unified Designer** (`/dashboard/designer`)
**NEW Purpose**: Complete menu design system with live data integration

#### **Left Panel: Data Sources**
```
┌─────────────────────┐
│ MENU DATA           │
│                     │
│ 📁 Categories       │
│  ├─ 🍕 Appetizers   │ ← Drag to canvas
│  ├─ 🍝 Main Courses │
│  ├─ 🍰 Desserts     │
│  └─ ☕ Beverages    │
│                     │
│ 📋 Restaurant Info  │
│  ├─ 🏪 Name         │ ← Drag to canvas
│  ├─ 🖼️ Logo         │
│  └─ 📞 Contact      │
│                     │
│ 🎨 Design Elements  │
│  ├─ ✏️ Text         │
│  ├─ 🖼️ Image        │
│  ├─ ⬛ Shape        │
│  └─ ➖ Line         │
└─────────────────────┘
```

#### **Center: Design Canvas**
```
┌───────────────────────────────┐
│  [Restaurant Name]            │ ← Live data element
│  [Logo]                       │ ← Live data element
│                               │
│  ─── APPETIZERS ───           │ ← Category element (dragged)
│   • Bruschetta.......$8       │ ← Auto-populated from DB
│   • Calamari.........$12      │
│   • Wings............$10      │
│                               │
│  ─── MAIN COURSES ───         │ ← Category element
│   • Pasta Carbonara..$18     │ ← Auto-populated from DB
│   • Grilled Salmon...$22     │
│                               │
│  [Contact: 123-456-7890]     │ ← Live data element
└───────────────────────────────┘
```

#### **Right Panel: Element Properties**
```
┌─────────────────────┐
│ PROPERTIES          │
│                     │
│ Element: Category   │
│ Type: Appetizers    │
│                     │
│ Layout Style:       │
│ ○ List              │
│ ● Grid (2 columns)  │
│ ○ Card              │
│                     │
│ Show:               │
│ ☑ Images            │
│ ☑ Descriptions      │
│ ☑ Prices            │
│                     │
│ Styling:            │
│ Background: #fff    │
│ Text Color: #333    │
│ Font Size: 16px     │
│ Spacing: 20px       │
└─────────────────────┘
```

---

### **3. How It Works**

#### **Step 1: Drag Category to Canvas**
```typescript
// User drags "Appetizers" category from left panel to canvas
{
  type: 'category',
  categoryId: 'cat-123',  // Links to real DB category
  x: 100,
  y: 200,
  layoutStyle: 'list',    // How items display
  showImages: true,
  showDescriptions: true,
  showPrices: true,
  styling: {
    backgroundColor: '#fff',
    textColor: '#333',
    fontSize: 16
  }
}

// Designer auto-fetches items from DB and renders them live
```

#### **Step 2: Customize Display**
- User selects category element
- Right panel shows layout options
- Choose: List, Grid, Card, Horizontal scroll
- Adjust: colors, fonts, spacing, borders
- Items update in real-time on canvas

#### **Step 3: Save Design**
```typescript
// Saves design configuration
MenuDesign {
  restaurantId: 'rest-123',
  isActive: true,  // This design is live on public menu
  elements: [
    {
      type: 'restaurantName',
      x: 50, y: 20,
      fontSize: 48, color: '#000'
    },
    {
      type: 'logo',
      x: 200, y: 20,
      width: 100, height: 100
    },
    {
      type: 'category',
      categoryId: 'cat-appetizers',
      x: 50, y: 150,
      layoutStyle: 'grid',
      columns: 2,
      styling: {...}
    },
    {
      type: 'category',
      categoryId: 'cat-mains',
      x: 50, y: 500,
      layoutStyle: 'list',
      styling: {...}
    }
  ]
}
```

#### **Step 4: Apply to Public Menu**
- Click "Publish" button
- Design becomes active menu layout
- Public menu (`/{slug}`) renders using this design
- Menu data (categories/items) is always current
- Design layout is fixed, data is dynamic

---

### **4. Remove/Enhance Templates**

#### **Option A: Remove Templates Entirely** ⭐ **RECOMMENDED**
**Reasoning**:
- Designer handles all layout needs
- No duplicate systems
- Simpler architecture
- Real-time design with live data

**Migration**:
- Remove `/dashboard/templates` page
- Remove template store
- Designer becomes sole design tool
- Provide "Design Presets" in designer instead

**Design Presets** (replacement for templates):
```
Designer → "Load Preset" button
├─ Classic List Layout
├─ Modern Grid Layout
├─ Elegant Card Layout
├─ Minimalist Layout
└─ Featured Items Layout
```

Each preset just configures element positions/styles, uses real data.

---

#### **Option B: Keep Templates as Quick Start** (Not Recommended)
**If you must keep templates**:
- Templates become "starting designs" for designer
- Click template → Opens designer with preset layout
- User customizes in designer
- Designer is still the primary tool

**Issues**:
- Still duplicates functionality
- More complex to maintain
- Users confused about difference

---

## 🎨 Designer Element Types

### **Live Data Elements** (Auto-update from DB)
1. **Restaurant Name**
   - Pulls from restaurant info
   - Updates automatically when changed in settings

2. **Logo**
   - Pulls from restaurant info
   - Updates automatically when logo changed

3. **Category**
   - Pulls category + all items from DB
   - Shows current items in real-time
   - Layout styles: list, grid, cards
   - Properties: show/hide images, prices, descriptions

4. **Contact Info**
   - Pulls from restaurant info
   - Phone, email, address

### **Static Design Elements**
5. **Text**
   - Custom text blocks
   - Headers, labels, descriptions

6. **Image**
   - Static images for decoration
   - Background images

7. **Shape**
   - Rectangles, circles, lines
   - Borders, dividers, backgrounds

---

## 💾 Database Schema Changes

### **New Table: `MenuDesigns`**
```sql
CREATE TABLE MenuDesigns (
  Id VARCHAR(36) PRIMARY KEY,
  RestaurantId VARCHAR(36) NOT NULL,
  Name VARCHAR(200),
  IsActive BOOLEAN DEFAULT false,
  DesignConfig JSON NOT NULL,  -- Contains all element configurations
  CreatedAt DATETIME,
  UpdatedAt DATETIME,
  FOREIGN KEY (RestaurantId) REFERENCES Restaurants(Id)
)
```

### **`DesignConfig` JSON Structure**
```json
{
  "width": 1080,
  "height": 1920,
  "backgroundColor": "#ffffff",
  "backgroundImage": null,
  "elements": [
    {
      "id": "elem-1",
      "type": "restaurantName",
      "x": 50,
      "y": 20,
      "width": 300,
      "height": 60,
      "styling": {
        "fontSize": 48,
        "fontFamily": "Playfair Display",
        "color": "#000",
        "textAlign": "center"
      }
    },
    {
      "id": "elem-2",
      "type": "category",
      "categoryId": "cat-123",
      "x": 50,
      "y": 150,
      "width": 980,
      "height": 400,
      "layoutStyle": "grid",
      "columns": 2,
      "gap": 20,
      "showImages": true,
      "showDescriptions": true,
      "showPrices": true,
      "styling": {
        "backgroundColor": "#f9f9f9",
        "textColor": "#333",
        "fontSize": 16,
        "padding": 20,
        "borderRadius": 8
      }
    }
  ]
}
```

---

## 🔄 Data Flow

### **Design Time** (Designer)
```
1. User opens /dashboard/designer
2. Load restaurant data (categories, items, info)
3. Display in left panel as draggable sources
4. User drags "Appetizers" category to canvas
5. Designer fetches current items from DB
6. Renders items in selected layout style
7. User customizes styling in right panel
8. Click "Save" → Stores design config to DB
9. Click "Publish" → Sets design as active
```

### **Public View** (Customer Menu)
```
1. Customer visits /{restaurantSlug}
2. Load active design config for restaurant
3. Load current categories and items
4. Render menu using design layout
5. Apply styling from design config
6. Items are always current from DB
```

### **When Restaurant Updates Menu**
```
1. Restaurant adds new item to "Appetizers"
2. Item saved to database
3. Public menu auto-updates (uses live data)
4. Design layout stays same
5. No need to republish design
```

---

## ✅ Benefits of New Architecture

### **For Restaurant Owners**
✅ **Single design tool** - No confusion between templates/designer
✅ **Live data preview** - See real menu while designing
✅ **Instant updates** - Menu changes appear immediately
✅ **Visual control** - Drag-and-drop everything
✅ **No technical skills** - Just drag and style

### **For Developers**
✅ **Single source of truth** - Designer is the design system
✅ **Simpler codebase** - Remove duplicate template system
✅ **Easier maintenance** - One system to maintain
✅ **Better data flow** - Clear separation: design config vs menu data
✅ **Scalable** - Easy to add new element types

### **For System**
✅ **Better performance** - No template processing
✅ **Real-time updates** - Menu always current
✅ **Flexible** - Any layout possible
✅ **Consistent** - Same design system everywhere

---

## 🚀 Implementation Plan

### **Phase 1: Core Designer Updates** (Week 1)
1. Add "Data Sources" left panel
2. Display categories from DB
3. Make categories draggable
4. Create "category" element type
5. Auto-fetch items when category dropped
6. Add layout style selector (list, grid, cards)

### **Phase 2: Live Data Integration** (Week 1)
1. Create `MenuDesigns` table
2. Build save/load design API
3. Add "Publish" functionality
4. Update public menu to use design config
5. Implement live data rendering

### **Phase 3: Settings Simplification** (Week 2)
1. Remove template selection from settings
2. Remove theme customization from settings
3. Keep only restaurant info
4. Add restaurant info as draggable elements

### **Phase 4: Template Migration** (Week 2)
1. Convert existing templates to design presets
2. Remove `/dashboard/templates` page
3. Remove template store
4. Add "Load Preset" to designer
5. Migration script for existing data

### **Phase 5: Polish & Testing** (Week 2)
1. Add more layout styles
2. Improve drag-and-drop UX
3. Add more styling options
4. Testing with real data
5. Documentation

---

## 📋 Final Recommendation

### **RECOMMENDED APPROACH**: ⭐

1. **Remove Templates System Entirely**
   - Replace with "Design Presets" in designer
   - Simpler, clearer architecture
   - One tool does everything

2. **Simplify Settings**
   - Restaurant info only
   - Remove theme/layout settings
   - Move everything to designer

3. **Unified Designer**
   - Live data integration
   - Drag categories from left panel
   - Choose layout styles
   - Publish directly to menu

4. **Benefits**:
   - ✅ Simpler system
   - ✅ Better UX
   - ✅ Easier to maintain
   - ✅ More powerful
   - ✅ Real-time updates

---

## 🤔 Questions to Consider

1. **Should we keep template page?**
   - My recommendation: **NO** - Use design presets instead

2. **What about existing templates?**
   - Convert to design presets
   - Provide migration script

3. **How to handle multiple designs?**
   - Restaurant can save multiple designs
   - Only one can be "active" (published)
   - Easy to switch between saved designs

4. **What if restaurant has no categories yet?**
   - Designer shows empty state
   - Link to create categories
   - Can still design with placeholder data

---

## 📊 Comparison

### **Current System** ❌
```
Templates Page → Create template → Apply to menu → Settings → Theme customization → Designer → Export design
```
**3 separate systems, no integration**

### **New System** ✅
```
Designer → Drag categories → Style layout → Publish → Live on public menu
```
**One unified system, fully integrated**

---

## 🎯 Next Steps

**Before implementing, we need to decide**:
1. ✅ Remove templates or keep as presets? → **Remove, use presets**
2. ✅ Which layout styles to support? → **List, Grid, Cards initially**
3. ✅ How to handle existing data? → **Migration script**
4. ✅ Timeline? → **2 weeks recommended**

**Please review and approve this architecture before I begin implementation.**

---

**Status**: 📝 **PROPOSAL - AWAITING APPROVAL**
**Date**: November 2025
**Estimated Implementation**: 2 weeks
**Impact**: High - Major architectural change
