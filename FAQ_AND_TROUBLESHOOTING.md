# ❓ FAQ & Troubleshooting Guide

## 📚 Table of Contents

1. [General Questions](#general-questions)
2. [Backend Questions](#backend-questions)
3. [Frontend Questions](#frontend-questions)
4. [Common Errors](#common-errors)
5. [Performance Issues](#performance-issues)
6. [Best Practices](#best-practices)

---

## 🤔 General Questions

### **Q: What is the Menu Generator system?**

**A:** The Menu Generator is a feature that allows restaurant owners to:
- Create reusable menu templates with custom themes
- Apply templates to generate complete menus instantly
- Customize colors, layouts, and display settings
- Provide branded, multi-language menus to customers via QR codes

---

### **Q: How does the theme system work?**

**A:** The theme system has three levels (priority order):

1. **Custom Theme** (Highest) - Restaurant-specific overrides
2. **Active Template Theme** (Medium) - Theme from selected template
3. **Default Theme** (Lowest) - System fallback

When a customer views the menu, the backend determines which theme to use and sends it to the frontend.

---

### **Q: Can I use templates from other restaurants?**

**A:** 
- **Global Templates** (created by admins) - Yes, everyone can use
- **Restaurant Templates** (created by owners) - Only the owner can use
- **Future Feature** - Template marketplace for sharing/selling templates

---

### **Q: What happens when I apply a template?**

**A:** When you apply a template:
1. Categories and items from the template are created in your restaurant
2. Translations are preserved
3. Your restaurant's `ActiveTemplateId` is updated
4. The template's `UsageCount` is incremented
5. If `overwriteExisting: true`, old menu items are removed first

---

### **Q: How do I update my menu after applying a template?**

**A:** After applying a template, you can:
1. Edit individual items in the Categories page
2. Add new items or categories
3. Change prices, descriptions, images
4. Update translations
5. Reorder items

The template is just a starting point - you have full control afterward.

---

## 🔧 Backend Questions

### **Q: How do I add a new property to the theme?**

**A:** Follow these steps:

1. **Update DTO:**
```csharp
// File: MenuTemplateDtos.cs
public class MenuTemplateThemeDto
{
    // ... existing properties ...
    public string MyNewProperty { get; set; } = "default";
}
```

2. **No migration needed** - Theme is stored as JSON, so it's flexible

3. **Update frontend interface:**
```typescript
// File: stores/templates.ts
interface TemplateTheme {
  // ... existing properties ...
  myNewProperty?: string
}
```

---

### **Q: How do I create a custom command/query?**

**A:** Follow the CQRS pattern:

**Command Example:**
```csharp
// 1. Create command
public record MyCommand(Guid Id, string Data) : IRequest<Result<MyDto>>;

// 2. Create handler
public class MyCommandHandler : IRequestHandler<MyCommand, Result<MyDto>>
{
    private readonly IApplicationDbContext _context;
    
    public MyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<MyDto>> Handle(MyCommand request, CancellationToken ct)
    {
        // Your logic here
        return Result<MyDto>.SuccessResult(data);
    }
}

// 3. Add to controller
[HttpPost("my-endpoint")]
public async Task<IActionResult> MyEndpoint([FromBody] MyDto dto)
{
    var result = await _mediator.Send(new MyCommand(dto.Id, dto.Data));
    return result.Success ? Ok(result) : BadRequest(result);
}
```

---

### **Q: How do I add a new entity?**

**A:** 

1. **Create entity:**
```csharp
// File: Menufy.Domain/Entities/MyEntity.cs
public class MyEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid RestaurantId { get; set; }
    
    // Navigation
    public Restaurant Restaurant { get; set; } = null!;
}
```

2. **Add to DbContext:**
```csharp
// File: ApplicationDbContext.cs
public DbSet<MyEntity> MyEntities { get; set; }
```

3. **Create migration:**
```bash
dotnet ef migrations add AddMyEntity
dotnet ef database update
```

---

### **Q: How do I handle JSON serialization?**

**A:**

**Serialize:**
```csharp
using System.Text.Json;

var theme = new MenuTemplateThemeDto { ... };
var json = JsonSerializer.Serialize(theme);
restaurant.CustomTheme = json;
```

**Deserialize:**
```csharp
var theme = JsonSerializer.Deserialize<MenuTemplateThemeDto>(restaurant.CustomTheme);
```

**With null handling:**
```csharp
var theme = restaurant.CustomTheme != null
    ? JsonSerializer.Deserialize<MenuTemplateThemeDto>(restaurant.CustomTheme)
    : null;
```

---

### **Q: How do I implement authorization?**

**A:**

**In Controller:**
```csharp
[Authorize(Roles = "RestaurantOwner,Admin")]
public class MyController : ControllerBase { }
```

**In Handler:**
```csharp
var isAdmin = string.Equals(request.UserRole, "Admin", StringComparison.OrdinalIgnoreCase);

if (!isAdmin && restaurant.OwnerId != request.UserId)
{
    return Result.FailureResult("Unauthorized");
}
```

---

## 🎨 Frontend Questions

### **Q: How do I add a new page?**

**A:**

1. **Create file:**
```bash
# File: pages/dashboard/my-page.vue
```

2. **Add content:**
```vue
<template>
  <div>
    <h1>My Page</h1>
  </div>
</template>

<script setup lang="ts">
// Your logic
</script>
```

3. **Add to navigation:**
```typescript
// File: layouts/dashboard.vue
const navItems = [
  // ... existing items ...
  { name: 'My Page', icon: 'icon', path: '/dashboard/my-page' }
]
```

---

### **Q: How do I call an API endpoint?**

**A:**

**Using $fetch (recommended):**
```typescript
const response = await $fetch('/api/menu-templates', {
  method: 'POST',
  body: {
    name: 'My Template',
    theme: { ... }
  }
})

if (response.success) {
  // Handle success
}
```

**With error handling:**
```typescript
try {
  const response = await $fetch('/api/menu-templates')
  if (response.success) {
    data.value = response.data
  }
} catch (error) {
  console.error('API Error:', error)
}
```

---

### **Q: How do I use Pinia stores?**

**A:**

**Access store:**
```typescript
import { useTemplatesStore } from '~/stores/templates'

const templatesStore = useTemplatesStore()
```

**Use state:**
```typescript
const templates = computed(() => templatesStore.templates)
```

**Call actions:**
```typescript
await templatesStore.fetchTemplates()
await templatesStore.createTemplate(data)
```

---

### **Q: How do I apply dynamic styles?**

**A:**

**Using :style:**
```vue
<template>
  <div :style="{
    '--primary-color': theme.primaryColor,
    '--accent-color': theme.accentColor
  }">
    <div class="bg-[var(--primary-color)]">Content</div>
  </div>
</template>
```

**Using :class:**
```vue
<template>
  <div :class="[
    theme.layout === 'grid' ? 'grid grid-cols-3' : 'flex flex-col',
    theme.spacing === 'compact' ? 'gap-2' : 'gap-6'
  ]">
    Content
  </div>
</template>
```

---

### **Q: How do I handle translations?**

**A:**

**In template:**
```vue
<template>
  <h1>{{ $t('dashboard.title') }}</h1>
  <p>{{ $t('dashboard.welcome', { name: userName }) }}</p>
</template>
```

**In script:**
```typescript
const { t } = useI18n()
const title = t('dashboard.title')
```

**Switch language:**
```typescript
const { locale } = useI18n()
locale.value = 'ar' // Switch to Arabic
```

---

## 🐛 Common Errors

### **Error: "Migration failed - column already exists"**

**Solution:**
```bash
# Drop and recreate database
dotnet ef database drop
dotnet ef database update
```

**Or manually remove the migration:**
```bash
dotnet ef migrations remove
# Then recreate it
dotnet ef migrations add EnhancedMenuGenerator
```

---

### **Error: "Cannot deserialize JSON"**

**Problem:** JSON structure doesn't match DTO

**Solution:**
```csharp
// Add null check
if (string.IsNullOrWhiteSpace(restaurant.CustomTheme))
{
    theme = null;
}
else
{
    try
    {
        theme = JsonSerializer.Deserialize<MenuTemplateThemeDto>(restaurant.CustomTheme);
    }
    catch (JsonException ex)
    {
        // Log error and use default
        theme = new MenuTemplateThemeDto();
    }
}
```

---

### **Error: "Unauthorized" when calling API**

**Problem:** Missing or invalid JWT token

**Solution:**

1. **Check if logged in:**
```typescript
const authStore = useAuthStore()
if (!authStore.isAuthenticated) {
  navigateTo('/login')
}
```

2. **Check token in request:**
```typescript
// Should be automatic, but verify in nuxt.config.ts
runtimeConfig: {
  public: {
    apiBase: process.env.NUXT_PUBLIC_API_BASE || 'https://localhost:5001'
  }
}
```

3. **Verify role:**
```csharp
// Backend
[Authorize(Roles = "RestaurantOwner,Admin")]
```

---

### **Error: "CORS policy blocked"**

**Problem:** Backend not allowing frontend origin

**Solution:**

**Backend - Program.cs:**
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ...

app.UseCors("AllowFrontend");
```

---

### **Error: "Cannot find module '@/components/...'"**

**Problem:** Import path issue

**Solution:**

Use `~/` for workspace root:
```typescript
// ❌ Wrong
import Button from '@/components/ui/Button.vue'

// ✅ Correct
import Button from '~/components/ui/Button.vue'
```

---

### **Error: "Theme not applying to public menu"**

**Problem:** Theme not being passed or applied correctly

**Solution:**

1. **Verify backend response:**
```bash
# Check API response includes theme
curl https://localhost:5001/api/menu/my-restaurant
```

2. **Verify frontend receives theme:**
```typescript
console.log('Theme:', menu.value?.theme)
```

3. **Verify styles are applied:**
```vue
<template>
  <div :style="themeStyles">
    <!-- Check in browser DevTools that CSS variables are set -->
  </div>
</template>

<script setup lang="ts">
const themeStyles = computed(() => {
  console.log('Applying theme:', menu.value?.theme)
  return {
    '--primary-color': menu.value?.theme?.primaryColor || '#dc2626'
  }
})
</script>
```

---

## ⚡ Performance Issues

### **Issue: Slow API responses**

**Solutions:**

1. **Add database indexes:**
```csharp
// In ApplicationDbContext.cs
modelBuilder.Entity<Restaurant>()
    .HasIndex(r => r.Slug);

modelBuilder.Entity<MenuCategory>()
    .HasIndex(c => c.RestaurantId);
```

2. **Use AsNoTracking for read-only queries:**
```csharp
var restaurants = await _context.Restaurants
    .AsNoTracking()
    .ToListAsync();
```

3. **Implement caching:**
```csharp
// Add Redis or in-memory cache
services.AddMemoryCache();

// In handler
var cacheKey = $"menu_{slug}";
if (!_cache.TryGetValue(cacheKey, out PublicMenuDto menu))
{
    menu = await LoadMenu(slug);
    _cache.Set(cacheKey, menu, TimeSpan.FromMinutes(5));
}
```

---

### **Issue: Large bundle size**

**Solutions:**

1. **Enable code splitting:**
```typescript
// nuxt.config.ts
export default defineNuxtConfig({
  build: {
    splitChunks: {
      layouts: true,
      pages: true,
      commons: true
    }
  }
})
```

2. **Lazy load components:**
```vue
<script setup lang="ts">
const ThemeCustomizer = defineAsyncComponent(() => 
  import('~/components/menu/ThemeCustomizer.vue')
)
</script>
```

3. **Optimize images:**
```vue
<template>
  <NuxtImg
    :src="item.imageUrl"
    format="webp"
    loading="lazy"
    width="300"
    height="200"
  />
</template>
```

---

### **Issue: Slow page load**

**Solutions:**

1. **Use SSR/SSG:**
```typescript
// nuxt.config.ts
export default defineNuxtConfig({
  ssr: true,
  
  routeRules: {
    '/menu/**': { swr: 3600 }, // Cache for 1 hour
    '/dashboard/**': { ssr: false } // Client-side only
  }
})
```

2. **Prefetch data:**
```vue
<script setup lang="ts">
// Prefetch on hover
const prefetchTemplate = (id: string) => {
  $fetch(`/api/menu-templates/${id}`)
}
</script>

<template>
  <div @mouseenter="prefetchTemplate(template.id)">
    {{ template.name }}
  </div>
</template>
```

---

## ✅ Best Practices

### **Backend Best Practices**

1. **Always use Result pattern:**
```csharp
// ✅ Good
return Result<MyDto>.SuccessResult(data, "Success message");
return Result<MyDto>.FailureResult("Error message");

// ❌ Bad
throw new Exception("Error");
```

2. **Validate input:**
```csharp
if (string.IsNullOrWhiteSpace(request.Name))
{
    return Result.FailureResult("Name is required");
}

if (request.Price < 0)
{
    return Result.FailureResult("Price must be positive");
}
```

3. **Use transactions for multiple operations:**
```csharp
using var transaction = await _context.Database.BeginTransactionAsync();
try
{
    // Multiple operations
    await _context.SaveChangesAsync();
    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

4. **Log important operations:**
```csharp
_logger.LogInformation("Template {TemplateId} applied to restaurant {RestaurantId}", 
    templateId, restaurantId);
```

---

### **Frontend Best Practices**

1. **Use composables for reusable logic:**
```typescript
// composables/useMenu.ts
export const useMenu = () => {
  const menu = ref(null)
  
  const loadMenu = async (slug: string) => {
    const response = await $fetch(`/api/menu/${slug}`)
    menu.value = response.data
  }
  
  return { menu, loadMenu }
}
```

2. **Handle loading states:**
```vue
<script setup lang="ts">
const loading = ref(false)
const error = ref(null)

const loadData = async () => {
  loading.value = true
  error.value = null
  try {
    // Load data
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div v-if="loading">Loading...</div>
  <div v-else-if="error">Error: {{ error }}</div>
  <div v-else>{{ data }}</div>
</template>
```

3. **Use TypeScript:**
```typescript
// ✅ Good
interface Template {
  id: string
  name: string
  theme: TemplateTheme
}

const template: Template = { ... }

// ❌ Bad
const template: any = { ... }
```

4. **Validate forms:**
```vue
<script setup lang="ts">
const form = ref({
  name: '',
  email: ''
})

const errors = ref({})

const validate = () => {
  errors.value = {}
  
  if (!form.value.name) {
    errors.value.name = 'Name is required'
  }
  
  if (!form.value.email.includes('@')) {
    errors.value.email = 'Invalid email'
  }
  
  return Object.keys(errors.value).length === 0
}

const submit = async () => {
  if (!validate()) return
  // Submit form
}
</script>
```

---

### **Database Best Practices**

1. **Use indexes on foreign keys:**
```sql
CREATE INDEX IX_MenuCategories_RestaurantId ON MenuCategories(RestaurantId);
CREATE INDEX IX_MenuItems_CategoryId ON MenuItems(CategoryId);
```

2. **Use appropriate data types:**
```csharp
// ✅ Good
public decimal Price { get; set; } // For money

// ❌ Bad
public float Price { get; set; } // Precision issues
```

3. **Normalize data:**
```csharp
// ✅ Good - Separate table for translations
public class MenuItemTranslation
{
    public Guid MenuItemId { get; set; }
    public string Language { get; set; }
    public string Name { get; set; }
}

// ❌ Bad - JSON blob (harder to query)
public string Translations { get; set; } // JSON
```

---

## 📞 Getting Help

### **Resources:**

1. **Documentation:**
   - `IMPLEMENTATION_SUMMARY.md` - Overview
   - `QUICK_IMPLEMENTATION_GUIDE.md` - Step-by-step guide
   - `ARCHITECTURE_FLOW.md` - System architecture
   - `backend/BACKEND_IMPLEMENTATION_GUIDE.md` - Backend details
   - `frontend/MENU_GENERATOR_ANALYSIS.md` - Frontend details

2. **Code Examples:**
   - Check existing handlers in `backend/src/Menufy.Application/Features/`
   - Check existing pages in `frontend/pages/`
   - Check existing components in `frontend/components/`

3. **Testing:**
   - Backend: `https://localhost:5001/swagger`
   - Frontend: `http://localhost:3000`

---

## 🎉 Success Checklist

Before deploying, verify:

- [ ] All migrations applied
- [ ] All tests passing
- [ ] No linter errors
- [ ] API documentation updated
- [ ] Frontend builds successfully
- [ ] Public menu displays correctly
- [ ] Theme customization works
- [ ] Template application works
- [ ] Settings save correctly
- [ ] QR codes generate
- [ ] Multi-language works
- [ ] Mobile responsive
- [ ] Performance acceptable
- [ ] Security reviewed
- [ ] Logs configured

---

This FAQ should help you solve most common issues! If you encounter something not covered here, check the detailed documentation files. 🚀

