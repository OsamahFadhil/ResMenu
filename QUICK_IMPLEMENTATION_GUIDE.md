# 🚀 Quick Implementation Guide - Menu Generator Enhancement

## ⚡ 15-Minute Quick Start

### **Backend (5 minutes)**

```bash
cd backend/src/Menufy.Domain/Entities
```

**1. Update `Restaurant.cs`** - Add these properties:

```csharp
public Guid? ActiveTemplateId { get; set; }
public string? CustomTheme { get; set; }
public string? MenuDisplaySettings { get; set; }
public string? Currency { get; set; } = "USD";
public string? DefaultLanguage { get; set; } = "en";
public int TotalMenuViews { get; set; } = 0;
public DateTime? LastMenuUpdate { get; set; }
public MenuTemplate? ActiveTemplate { get; set; }
```

**2. Update `MenuTemplate.cs`** - Add these properties:

```csharp
public int UsageCount { get; set; } = 0;
public DateTime? LastUsedAt { get; set; }
```

**3. Create Migration:**

```bash
cd ../Menufy.Infrastructure
dotnet ef migrations add EnhancedMenuGenerator
dotnet ef database update
```

---

### **Frontend (10 minutes)**

**1. Update Templates Page** - `frontend/pages/dashboard/templates.vue`

Add after the description field in the create/edit modal:

```vue
<!-- Theme Customization -->
<div class="space-y-4">
  <h3 class="text-lg font-semibold text-neutral-900">Theme Settings</h3>
  <ThemeCustomizer v-model="form.theme" />
  <LayoutCustomizer v-model="form.theme" />
</div>
```

Add imports at top:

```vue
<script setup lang="ts">
import ThemeCustomizer from '~/components/menu/ThemeCustomizer.vue'
import LayoutCustomizer from '~/components/menu/LayoutCustomizer.vue'
// ... existing imports
</script>
```

**2. Test:**

```bash
cd frontend
npm run dev
```

Visit: `http://localhost:3000/dashboard/templates`

---

## 📝 Complete Backend Implementation (30 minutes)

### **Step 1: Create DTOs** (5 min)

**File:** `backend/src/Menufy.Application/Features/Restaurants/DTOs/RestaurantSettingsDto.cs`

```csharp
namespace Menufy.Application.Features.Restaurants.DTOs;

public class RestaurantSettingsDto
{
    public Guid RestaurantId { get; set; }
    public Guid? ActiveTemplateId { get; set; }
    public MenuTemplateThemeDto? CustomTheme { get; set; }
    public MenuDisplaySettingsDto DisplaySettings { get; set; } = new();
    public string Currency { get; set; } = "USD";
    public string DefaultLanguage { get; set; } = "en";
}

public class MenuDisplaySettingsDto
{
    public bool ShowPrices { get; set; } = true;
    public bool ShowImages { get; set; } = true;
    public bool ShowDescriptions { get; set; } = true;
    public bool ShowCategories { get; set; } = true;
    public bool EnableSearch { get; set; } = true;
    public bool EnableFilters { get; set; } = true;
    public List<string> AvailableLanguages { get; set; } = new() { "en", "ar" };
}

public class UpdateRestaurantSettingsDto
{
    public Guid? ActiveTemplateId { get; set; }
    public MenuTemplateThemeDto? CustomTheme { get; set; }
    public MenuDisplaySettingsDto? DisplaySettings { get; set; }
    public string? Currency { get; set; }
    public string? DefaultLanguage { get; set; }
}
```

### **Step 2: Update Theme DTO** (5 min)

**File:** `backend/src/Menufy.Application/Features/MenuTemplates/DTOs/MenuTemplateDtos.cs`

Replace `MenuTemplateThemeDto` with:

```csharp
public class MenuTemplateThemeDto
{
    // Colors
    public string PrimaryColor { get; set; } = "#dc2626";
    public string AccentColor { get; set; } = "#f59e0b";
    public string BackgroundColor { get; set; } = "#fafaf9";
    public string SurfaceColor { get; set; } = "#ffffff";
    public string TextColor { get; set; } = "#292524";
    
    // Typography
    public string FontFamily { get; set; } = "Inter";
    public string? HeadingFont { get; set; }
    public string? BodyFont { get; set; }
    public string FontSize { get; set; } = "medium";
    
    // Layout
    public string Layout { get; set; } = "list";
    public string CardStyle { get; set; } = "modern";
    public string BorderRadius { get; set; } = "medium";
    
    // Spacing
    public string Spacing { get; set; } = "normal";
    
    // Images
    public bool ShowImages { get; set; } = true;
    public string ImageSize { get; set; } = "medium";
    public string ImageShape { get; set; } = "rounded";
    
    // Branding
    public string LogoPosition { get; set; } = "left";
    public bool ShowRestaurantInfo { get; set; } = true;
    public string HeaderStyle { get; set; } = "simple";
}
```

### **Step 3: Create Commands** (10 min)

**File:** `backend/src/Menufy.Application/Features/Restaurants/Commands/UpdateSettings/UpdateRestaurantSettingsCommand.cs`

```csharp
using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Restaurants.DTOs;

namespace Menufy.Application.Features.Restaurants.Commands.UpdateSettings;

public record UpdateRestaurantSettingsCommand(
    Guid RestaurantId,
    UpdateRestaurantSettingsDto Settings
) : IRequest<Result<RestaurantSettingsDto>>;
```

**File:** `UpdateRestaurantSettingsCommandHandler.cs`

```csharp
using System.Text.Json;
using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.MenuTemplates.DTOs;
using Menufy.Application.Features.Restaurants.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.Restaurants.Commands.UpdateSettings;

public class UpdateRestaurantSettingsCommandHandler 
    : IRequestHandler<UpdateRestaurantSettingsCommand, Result<RestaurantSettingsDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateRestaurantSettingsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<RestaurantSettingsDto>> Handle(
        UpdateRestaurantSettingsCommand request, 
        CancellationToken cancellationToken)
    {
        var restaurant = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken);

        if (restaurant == null)
            return Result<RestaurantSettingsDto>.FailureResult("Restaurant not found");

        if (request.Settings.ActiveTemplateId.HasValue)
            restaurant.ActiveTemplateId = request.Settings.ActiveTemplateId;

        if (request.Settings.CustomTheme != null)
            restaurant.CustomTheme = JsonSerializer.Serialize(request.Settings.CustomTheme);

        if (request.Settings.DisplaySettings != null)
            restaurant.MenuDisplaySettings = JsonSerializer.Serialize(request.Settings.DisplaySettings);

        if (!string.IsNullOrWhiteSpace(request.Settings.Currency))
            restaurant.Currency = request.Settings.Currency;

        if (!string.IsNullOrWhiteSpace(request.Settings.DefaultLanguage))
            restaurant.DefaultLanguage = request.Settings.DefaultLanguage;

        restaurant.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        var response = new RestaurantSettingsDto
        {
            RestaurantId = restaurant.Id,
            ActiveTemplateId = restaurant.ActiveTemplateId,
            CustomTheme = restaurant.CustomTheme != null 
                ? JsonSerializer.Deserialize<MenuTemplateThemeDto>(restaurant.CustomTheme) 
                : null,
            DisplaySettings = restaurant.MenuDisplaySettings != null
                ? JsonSerializer.Deserialize<MenuDisplaySettingsDto>(restaurant.MenuDisplaySettings)!
                : new MenuDisplaySettingsDto(),
            Currency = restaurant.Currency ?? "USD",
            DefaultLanguage = restaurant.DefaultLanguage ?? "en"
        };

        return Result<RestaurantSettingsDto>.SuccessResult(response, "Settings updated");
    }
}
```

### **Step 4: Create Query** (5 min)

**File:** `backend/src/Menufy.Application/Features/Restaurants/Queries/GetSettings/GetRestaurantSettingsQuery.cs`

```csharp
using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Restaurants.DTOs;

namespace Menufy.Application.Features.Restaurants.Queries.GetSettings;

public record GetRestaurantSettingsQuery(Guid RestaurantId) 
    : IRequest<Result<RestaurantSettingsDto>>;
```

**File:** `GetRestaurantSettingsQueryHandler.cs`

```csharp
using System.Text.Json;
using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.MenuTemplates.DTOs;
using Menufy.Application.Features.Restaurants.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.Restaurants.Queries.GetSettings;

public class GetRestaurantSettingsQueryHandler 
    : IRequestHandler<GetRestaurantSettingsQuery, Result<RestaurantSettingsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRestaurantSettingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<RestaurantSettingsDto>> Handle(
        GetRestaurantSettingsQuery request, 
        CancellationToken cancellationToken)
    {
        var restaurant = await _context.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken);

        if (restaurant == null)
            return Result<RestaurantSettingsDto>.FailureResult("Restaurant not found");

        var settings = new RestaurantSettingsDto
        {
            RestaurantId = restaurant.Id,
            ActiveTemplateId = restaurant.ActiveTemplateId,
            CustomTheme = restaurant.CustomTheme != null
                ? JsonSerializer.Deserialize<MenuTemplateThemeDto>(restaurant.CustomTheme)
                : null,
            DisplaySettings = restaurant.MenuDisplaySettings != null
                ? JsonSerializer.Deserialize<MenuDisplaySettingsDto>(restaurant.MenuDisplaySettings)!
                : new MenuDisplaySettingsDto(),
            Currency = restaurant.Currency ?? "USD",
            DefaultLanguage = restaurant.DefaultLanguage ?? "en"
        };

        return Result<RestaurantSettingsDto>.SuccessResult(settings);
    }
}
```

### **Step 5: Create Controller** (5 min)

**File:** `backend/src/Menufy.API/Controllers/RestaurantSettingsController.cs`

```csharp
using MediatR;
using Menufy.Application.Features.Restaurants.Commands.UpdateSettings;
using Menufy.Application.Features.Restaurants.DTOs;
using Menufy.Application.Features.Restaurants.Queries.GetSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Menufy.API.Controllers;

[ApiController]
[Route("api/restaurants/{restaurantId:guid}")]
[Authorize(Roles = "RestaurantOwner,Admin")]
public class RestaurantSettingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RestaurantSettingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings(Guid restaurantId)
    {
        var result = await _mediator.Send(new GetRestaurantSettingsQuery(restaurantId));
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSettings(
        Guid restaurantId, 
        [FromBody] UpdateRestaurantSettingsDto dto)
    {
        var result = await _mediator.Send(
            new UpdateRestaurantSettingsCommand(restaurantId, dto));
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
```

---

## 🎨 Complete Frontend Implementation (45 minutes)

### **Step 1: Create Settings Page** (15 min)

**File:** `frontend/pages/dashboard/settings.vue`

```vue
<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-2xl font-bold text-neutral-900">Restaurant Settings</h1>
      <p class="text-neutral-600 mt-1">Manage your menu appearance and preferences</p>
    </div>

    <!-- Active Template -->
    <Card>
      <template #header>
        <h2 class="text-lg font-semibold">Active Template</h2>
      </template>
      <template #body>
        <Dropdown
          v-model="settings.activeTemplateId"
          :options="templateOptions"
          placeholder="Select a template"
        />
      </template>
    </Card>

    <!-- Custom Theme -->
    <Card>
      <template #header>
        <h2 class="text-lg font-semibold">Custom Theme</h2>
      </template>
      <template #body>
        <ThemeCustomizer v-model="settings.customTheme" />
        <LayoutCustomizer v-model="settings.customTheme" class="mt-6" />
      </template>
    </Card>

    <!-- Display Settings -->
    <Card>
      <template #header>
        <h2 class="text-lg font-semibold">Display Settings</h2>
      </template>
      <template #body>
        <div class="space-y-4">
          <label class="flex items-center gap-3">
            <input type="checkbox" v-model="settings.displaySettings.showPrices" class="rounded" />
            <span>Show Prices</span>
          </label>
          <label class="flex items-center gap-3">
            <input type="checkbox" v-model="settings.displaySettings.showImages" class="rounded" />
            <span>Show Images</span>
          </label>
          <label class="flex items-center gap-3">
            <input type="checkbox" v-model="settings.displaySettings.enableSearch" class="rounded" />
            <span>Enable Search</span>
          </label>
        </div>
      </template>
    </Card>

    <!-- Save Button -->
    <div class="flex justify-end">
      <Button @click="saveSettings" :loading="saving">
        Save Settings
      </Button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRestaurantStore } from '~/stores/restaurant'
import ThemeCustomizer from '~/components/menu/ThemeCustomizer.vue'
import LayoutCustomizer from '~/components/menu/LayoutCustomizer.vue'

const restaurantStore = useRestaurantStore()
const saving = ref(false)

const settings = ref({
  activeTemplateId: null,
  customTheme: {},
  displaySettings: {
    showPrices: true,
    showImages: true,
    showDescriptions: true,
    enableSearch: true,
    enableFilters: true,
  },
  currency: 'USD',
  defaultLanguage: 'en',
})

const templateOptions = ref([])

onMounted(async () => {
  // Load current settings
  const response = await $fetch(`/api/restaurants/${restaurantStore.currentRestaurant.id}/settings`)
  if (response.success) {
    settings.value = response.data
  }

  // Load templates
  const templatesResponse = await $fetch('/api/menu-templates')
  if (templatesResponse.success) {
    templateOptions.value = templatesResponse.data.map(t => ({
      value: t.id,
      label: t.name
    }))
  }
})

const saveSettings = async () => {
  saving.value = true
  try {
    const response = await $fetch(
      `/api/restaurants/${restaurantStore.currentRestaurant.id}/settings`,
      {
        method: 'PUT',
        body: settings.value
      }
    )
    if (response.success) {
      // Show success message
    }
  } finally {
    saving.value = false
  }
}
</script>
```

### **Step 2: Update Public Menu** (15 min)

**File:** `frontend/pages/menu/[slug].vue`

Add computed styles:

```vue
<script setup lang="ts">
// ... existing code ...

const themeStyles = computed(() => {
  if (!menu.value?.theme) return {}
  
  const theme = menu.value.theme
  return {
    '--primary-color': theme.primaryColor,
    '--accent-color': theme.accentColor,
    '--background-color': theme.backgroundColor,
    '--surface-color': theme.surfaceColor,
    '--text-color': theme.textColor,
    '--font-family': theme.fontFamily,
  }
})
</script>

<template>
  <div class="min-h-screen" :style="themeStyles">
    <!-- Apply theme classes dynamically -->
    <div :class="[
      'bg-[var(--background-color)]',
      'text-[var(--text-color)]',
      `font-[var(--font-family)]`
    ]">
      <!-- Rest of template -->
    </div>
  </div>
</template>
```

### **Step 3: Add to Navigation** (5 min)

**File:** `frontend/layouts/dashboard.vue`

Add to navigation items:

```typescript
const navItems = [
  { name: 'Dashboard', icon: 'home', path: '/dashboard' },
  { name: 'Categories', icon: 'folder', path: '/dashboard/categories' },
  { name: 'Templates', icon: 'template', path: '/dashboard/templates' },
  { name: 'Settings', icon: 'settings', path: '/dashboard/settings' }, // NEW
  { name: 'QR Code', icon: 'qrcode', path: '/dashboard/qr-code' },
]
```

### **Step 4: Test Everything** (10 min)

```bash
# Start backend
cd backend/src/Menufy.API
dotnet run

# Start frontend
cd frontend
npm run dev

# Test:
# 1. Create a template with custom theme
# 2. Go to settings and select the template
# 3. Customize colors and layout
# 4. Save settings
# 5. View public menu - should reflect changes
```

---

## ✅ Testing Checklist

### **Backend:**
- [ ] Migration applied successfully
- [ ] Can create template with enhanced theme
- [ ] Can get restaurant settings
- [ ] Can update restaurant settings
- [ ] Public menu includes theme data
- [ ] Swagger docs updated

### **Frontend:**
- [ ] Theme customizer works
- [ ] Layout customizer works
- [ ] Settings page loads
- [ ] Can save settings
- [ ] Public menu applies theme
- [ ] No console errors

---

## 🐛 Common Issues & Fixes

### **Issue: Migration fails**
```bash
# Fix: Reset and recreate
dotnet ef database drop
dotnet ef database update
```

### **Issue: Frontend can't connect to API**
```typescript
// Check nuxt.config.ts
runtimeConfig: {
  public: {
    apiBase: 'https://localhost:5001'
  }
}
```

### **Issue: Theme not applying**
```vue
<!-- Make sure to use :style binding -->
<div :style="themeStyles">
  <!-- content -->
</div>
```

---

## 📚 Full Documentation

For complete details, see:
- `IMPLEMENTATION_SUMMARY.md` - Overview
- `frontend/MENU_GENERATOR_ANALYSIS.md` - Frontend deep dive
- `backend/BACKEND_IMPLEMENTATION_GUIDE.md` - Backend details

---

## 🎉 You're Ready!

Follow the steps above and you'll have a fully functional enhanced menu generator in about 45 minutes!

**Questions?** Check the full documentation files for detailed explanations.

