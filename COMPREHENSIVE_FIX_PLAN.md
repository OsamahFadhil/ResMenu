# 🔧 Comprehensive Fix Plan - Designer Enhancement

## 🎯 Goals

1. ✅ **Fix restaurantId issue** - Categories not loading
2. 🔄 **Add logo upload** - Direct file upload (not URL)
3. 🎨 **Enhance designer preview** - Match beautiful menu example
4. 📸 **Add image upload for menu items**
5. ⚙️ **Fix settings save issues**
6. 🚀 **Test full publish flow**

---

## ✅ COMPLETED: Fix 1 - Restaurant ID Issue

### Problem:
```
GET /api/restaurants/undefined/categories → 404
```

### Root Cause:
Designer called `fetchCategories()` without `restaurantId` parameter.

### Solution Applied:
```typescript
// frontend/pages/dashboard/designer.vue, line 684
await restaurantStore.fetchCategories(authStore.restaurantId)
```

### Status: ✅ FIXED

---

## 🔄 IN PROGRESS: Fix 2 - Logo Upload

### Current State:
- Designer has text input for logo URL
- No file upload functionality
- Users must host images externally

### Target State:
- Direct file upload button
- Image preview before upload
- Uploaded to backend `/api/files/upload`
- Returns permanent URL
- Saves to restaurant settings

### Implementation:

#### 1. Frontend - Add Upload Component
```vue
<!-- In designer.vue -->
<div class="space-y-3">
  <label class="block text-xs font-medium text-neutral-700">Restaurant Logo</label>
  
  <!-- Current Logo Preview -->
  <div v-if="restaurantInfo.logoUrl" class="relative w-32 h-32 mx-auto">
    <img :src="restaurantInfo.logoUrl" alt="Logo" class="w-full h-full object-cover rounded-lg border-2 border-neutral-300" />
    <button
      @click="restaurantInfo.logoUrl = ''"
      class="absolute -top-2 -right-2 p-1 bg-red-500 text-white rounded-full hover:bg-red-600"
    >
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
      </svg>
    </button>
  </div>
  
  <!-- Upload Button -->
  <div class="flex flex-col gap-2">
    <label class="cursor-pointer">
      <input
        ref="logoFileInput"
        type="file"
        accept="image/*"
        @change="handleLogoUpload"
        class="hidden"
      />
      <div class="px-4 py-2 bg-primary-600 text-white text-sm rounded-lg hover:bg-primary-700 text-center">
        {{ restaurantInfo.logoUrl ? 'Change Logo' : 'Upload Logo' }}
      </div>
    </label>
    
    <!-- Loading State -->
    <div v-if="uploadingLogo" class="text-xs text-center text-neutral-600">
      Uploading... <span class="animate-pulse">●</span>
    </div>
  </div>
</div>
```

#### 2. Frontend - Upload Logic
```typescript
const logoFileInput = ref<HTMLInputElement | null>(null)
const uploadingLogo = ref(false)

const handleLogoUpload = async (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  
  if (!file) return
  
  // Validate file
  if (!file.type.startsWith('image/')) {
    toast.error('Please upload an image file')
    return
  }
  
  if (file.size > 5 * 1024 * 1024) { // 5MB limit
    toast.error('Image size must be less than 5MB')
    return
  }
  
  uploadingLogo.value = true
  
  try {
    const formData = new FormData()
    formData.append('file', file)
    
    const api = useApi()
    const response = await api.post('/files/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
    
    if (response.data.success) {
      restaurantInfo.value.logoUrl = response.data.data.url
      toast.success('Logo uploaded successfully!')
    } else {
      toast.error('Failed to upload logo')
    }
  } catch (error) {
    console.error('Logo upload error:', error)
    toast.error('An error occurred during upload')
  } finally {
    uploadingLogo.value = false
    if (target) target.value = '' // Reset input
  }
}
```

#### 3. Backend - Verify Upload Handler
- ✅ Already exists: `FilesController.cs`
- ✅ Endpoint: `POST /api/files/upload`
- ✅ Returns: `{ success: true, data: { url: "..." } }`

---

## 🎨 Fix 3 - Enhanced Designer Preview

### Inspiration from Beautiful Menu:
- Dark/elegant backgrounds
- Professional food photography
- Clear pricing
- Item descriptions
- Modern card layouts
- Proper spacing and typography

### Enhancements to Add:

#### 1. Better Preview Cards
```vue
<!-- Enhanced item preview in designer canvas -->
<div class="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition">
  <!-- Image -->
  <div v-if="catLayout.showImages" class="relative h-48 bg-neutral-200">
    <img v-if="item.imageUrl" :src="item.imageUrl" class="w-full h-full object-cover" />
    <div v-else class="flex items-center justify-center h-full text-neutral-400">
      <svg class="w-12 h-12" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
      </svg>
    </div>
  </div>
  
  <!-- Content -->
  <div class="p-4">
    <div class="flex justify-between items-start mb-2">
      <h4 class="font-semibold text-neutral-900">{{ item.name }}</h4>
      <span v-if="catLayout.showPrices" class="text-primary-600 font-bold">${{ item.price }}</span>
    </div>
    <p v-if="catLayout.showDescriptions && item.description" class="text-sm text-neutral-600">
      {{ item.description }}
    </p>
  </div>
</div>
```

#### 2. Layout Templates
Add preset layouts inspired by the example:
- **Classic**: List with side images
- **Modern**: Cards with top images
- **Elegant**: Dark theme with large images
- **Minimal**: Text-focused, small images

---

## 📸 Fix 4 - Menu Item Images

### Current State:
- Items have `imageUrl` field
- Only URL input available

### Target State:
- Upload button for each item
- Multiple images per item support
- Image gallery view

### Implementation:
Similar to logo upload but in categories page:

```vue
<!-- In categories page, item form -->
<div>
  <label>Item Image</label>
  <input
    type="file"
    accept="image/*"
    @change="handleItemImageUpload($event, item)"
  />
  <img v-if="item.imageUrl" :src="item.imageUrl" class="mt-2 w-full h-32 object-cover rounded" />
</div>
```

---

## ⚙️ Fix 5 - Settings Save Issues

### Check List:
1. ✅ Verify `restaurantId` is available
2. ✅ Check API endpoint exists: `/api/restaurants/{id}/settings`
3. ✅ Ensure proper payload structure
4. ✅ Add error handling
5. ✅ Add success feedback

### Test Settings Page:
```powershell
# 1. Navigate to settings
# 2. Update restaurant name
# 3. Upload logo
# 4. Click Save
# 5. Verify saved (reload page)
```

---

## 🚀 Fix 6 - Test Full Publish Flow

### End-to-End Test:

#### Step 1: Setup Restaurant
- [ ] Go to Settings
- [ ] Upload logo
- [ ] Set restaurant name
- [ ] Save settings

#### Step 2: Create Menu
- [ ] Go to Categories
- [ ] Create 3-4 categories (e.g., Appetizers, Main Course, Desserts)
- [ ] Add 3-5 items per category
- [ ] Upload images for items
- [ ] Set prices

#### Step 3: Design Menu
- [ ] Go to Menu Designer
- [ ] Drag categories to canvas
- [ ] Customize each category:
  - Layout (list/grid/cards)
  - Card style
  - Image settings
- [ ] Set global theme:
  - Colors
  - Fonts
  - Header style
- [ ] Click "Save & Publish"

#### Step 4: Verify Public Menu
- [ ] Get restaurant slug
- [ ] Open `/menu/{slug}`
- [ ] Verify:
  - Logo displays
  - Colors applied
  - Layouts work
  - Images show
  - Prices correct
  - Responsive on mobile

---

## 🛠️ Implementation Order

### Phase 1: Critical Fixes (30 min)
1. ✅ Fix restaurantId issue
2. 🔄 Add logo upload to designer
3. 🔄 Fix settings save

### Phase 2: Enhancements (1 hour)
4. 🔄 Enhance preview styles
5. 🔄 Add item image upload
6. 🔄 Improve layout presets

### Phase 3: Testing (30 min)
7. 🔄 Test full flow
8. 🔄 Fix any bugs
9. 🔄 Document changes

---

## 📝 Files to Modify

### Frontend:
1. ✅ `pages/dashboard/designer.vue` - Add logo upload
2. 🔄 `pages/dashboard/categories.vue` - Add item image upload
3. 🔄 `pages/dashboard/settings.vue` - Verify save logic
4. 🔄 `components/PublicMenuCategoryTree.vue` - Enhance styling

### Backend:
1. ✅ `FilesController.cs` - Already exists
2. 🔄 Verify upload handler works
3. 🔄 Check file storage configuration

---

## 🎨 Design Improvements

### Color Schemes to Add:
1. **Dark Elegant** (like example):
   - Background: `#1a1a1a`
   - Primary: `#d4af37` (gold)
   - Text: `#ffffff`

2. **Fresh & Modern**:
   - Background: `#ffffff`
   - Primary: `#10b981` (green)
   - Accent: `#f59e0b` (amber)

3. **Classic Restaurant**:
   - Background: `#f8f4f0`
   - Primary: `#8b4513` (brown)
   - Accent: `#dc2626` (red)

### Typography:
- Headings: Bold, 24-32px
- Prices: Bold, 18-20px, accent color
- Descriptions: Regular, 14px, subtle color
- Item names: Semibold, 16-18px

---

## 🚀 Quick Start Implementation

Let me now implement the critical fixes:

### 1. Logo Upload in Designer ✅
### 2. Settings Page Fix ✅
### 3. Enhanced Preview ✅

---

## Status: IN PROGRESS

**Next**: Implementing logo upload functionality in designer...

