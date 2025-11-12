# Backend Implementation Guide - Enhanced Menu Generator

## 📋 Table of Contents
1. [Current Backend Architecture](#current-backend-architecture)
2. [Required Database Changes](#required-database-changes)
3. [Entity Updates](#entity-updates)
4. [DTO Updates](#dto-updates)
5. [New Commands & Queries](#new-commands--queries)
6. [API Endpoints](#api-endpoints)
7. [Implementation Steps](#implementation-steps)

---

## 🏗️ Current Backend Architecture

### Existing Structure

```
backend/
├── Menufy.Domain/
│   └── Entities/
│       ├── MenuTemplate.cs          ✅ Exists
│       ├── MenuCategory.cs          ✅ Exists
│       ├── MenuItem.cs              ✅ Exists
│       ├── Restaurant.cs            ✅ Exists
│       └── QRCode.cs                ✅ Exists
│
├── Menufy.Application/
│   └── Features/
│       ├── MenuTemplates/           ✅ Exists
│       │   ├── Commands/
│       │   │   ├── CreateMenuTemplate/
│       │   │   ├── UpdateMenuTemplate/
│       │   │   └── DeleteMenuTemplate/
│       │   ├── Queries/
│       │   │   ├── GetMenuTemplates/
│       │   │   └── GetMenuTemplateById/
│       │   ├── DTOs/
│       │   └── Helpers/
│       │
│       └── Menus/                   ✅ Exists
│           └── Commands/
│               └── GenerateMenu/    ✅ Exists
│
└── Menufy.API/
    └── Controllers/
        ├── MenuTemplatesController.cs ✅ Exists
        └── MenusController.cs         ✅ Exists
```

### Current Entities

**MenuTemplate** (Existing):
```csharp
public class MenuTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? RestaurantId { get; set; }
    public string Structure { get; set; } = string.Empty;  // JSON
    public string? Theme { get; set; }                      // JSON
    public string? Tags { get; set; }                       // JSON
    public bool IsPublished { get; set; }
    
    public Restaurant? Restaurant { get; set; }
}
```

**Restaurant** (Existing):
```csharp
public class Restaurant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? Address { get; set; }
    public string? Translations { get; set; }
    public Guid OwnerId { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation
    public User Owner { get; set; } = null!;
    public ICollection<MenuCategory> MenuCategories { get; set; }
}
```

---

## 🗄️ Required Database Changes

### 1. Update MenuTemplate Entity

**File:** `backend/src/Menufy.Domain/Entities/MenuTemplate.cs`

```csharp
using Menufy.Domain.Common;

namespace Menufy.Domain.Entities;

public class MenuTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? RestaurantId { get; set; }
    
    // JSON Storage
    public string Structure { get; set; } = string.Empty;  // Categories & Items
    public string? Theme { get; set; }                      // Theme settings (ENHANCED)
    public string? Tags { get; set; }                       // Template tags
    
    // Metadata
    public bool IsPublished { get; set; }
    public int UsageCount { get; set; } = 0;                // NEW: Track usage
    public DateTime? LastUsedAt { get; set; }               // NEW: Last generation time
    
    // Navigation
    public Restaurant? Restaurant { get; set; }
}
```

### 2. Update Restaurant Entity

**File:** `backend/src/Menufy.Domain/Entities/Restaurant.cs`

Add new properties for menu customization:

```csharp
public class Restaurant : BaseEntity
{
    // ... existing properties ...
    
    // NEW: Menu Configuration
    public Guid? ActiveTemplateId { get; set; }             // Currently active template
    public string? CustomTheme { get; set; }                // JSON: Override theme settings
    public string? MenuDisplaySettings { get; set; }        // JSON: Display preferences
    public string? Currency { get; set; } = "USD";          // Default currency
    public string? DefaultLanguage { get; set; } = "en";    // Default menu language
    
    // NEW: Analytics
    public int TotalMenuViews { get; set; } = 0;
    public DateTime? LastMenuUpdate { get; set; }
    
    // Navigation
    public MenuTemplate? ActiveTemplate { get; set; }
}
```

### 3. Create Migration

```bash
cd backend/src/Menufy.Infrastructure
dotnet ef migrations add EnhancedMenuGenerator
dotnet ef database update
```

---

## 📦 DTO Updates

### 1. Enhanced Theme DTO

**File:** `backend/src/Menufy.Application/Features/MenuTemplates/DTOs/MenuTemplateDtos.cs`

Update `MenuTemplateThemeDto`:

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
    public string FontSize { get; set; } = "medium"; // small, medium, large
    
    // Layout
    public string Layout { get; set; } = "list"; // list, grid, cards
    public string CardStyle { get; set; } = "modern"; // modern, classic, minimal
    public string BorderRadius { get; set; } = "medium"; // none, small, medium, large
    
    // Spacing
    public string Spacing { get; set; } = "normal"; // compact, normal, relaxed
    
    // Images
    public bool ShowImages { get; set; } = true;
    public string ImageSize { get; set; } = "medium"; // small, medium, large
    public string ImageShape { get; set; } = "rounded"; // square, rounded, circle
    
    // Branding
    public string LogoPosition { get; set; } = "left"; // left, center, right
    public bool ShowRestaurantInfo { get; set; } = true;
    public string HeaderStyle { get; set; } = "simple"; // simple, banner, overlay
}
```

### 2. Restaurant Settings DTO

**File:** `backend/src/Menufy.Application/Features/Restaurants/DTOs/RestaurantSettingsDto.cs` (NEW)

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

### 3. Enhanced Public Menu DTO

**File:** `backend/src/Menufy.Application/Features/Menus/DTOs/PublicMenuDto.cs`

Update to include theme:

```csharp
public class PublicMenuDto
{
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public string RestaurantLocalizedName { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? Address { get; set; }
    public string Language { get; set; } = "en";
    
    // NEW: Theme & Settings
    public MenuTemplateThemeDto? Theme { get; set; }
    public MenuDisplaySettingsDto? DisplaySettings { get; set; }
    public string Currency { get; set; } = "USD";
    
    public List<MenuCategoryDto> Categories { get; set; } = new();
}
```

---

## 🔧 New Commands & Queries

### 1. Apply Template Command

**File:** `backend/src/Menufy.Application/Features/Restaurants/Commands/ApplyTemplate/ApplyTemplateCommand.cs` (NEW)

```csharp
using MediatR;
using Menufy.Application.Common.Models;

namespace Menufy.Application.Features.Restaurants.Commands.ApplyTemplate;

public record ApplyTemplateCommand(
    Guid RestaurantId,
    Guid TemplateId,
    bool OverwriteExisting = false
) : IRequest<Result<ApplyTemplateResultDto>>;

public class ApplyTemplateResultDto
{
    public Guid RestaurantId { get; set; }
    public Guid TemplateId { get; set; }
    public int CategoriesCreated { get; set; }
    public int ItemsCreated { get; set; }
    public DateTime AppliedAt { get; set; }
}
```

**Handler:** `ApplyTemplateCommandHandler.cs`

```csharp
using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Menus.Commands.GenerateMenu;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.Restaurants.Commands.ApplyTemplate;

public class ApplyTemplateCommandHandler : IRequestHandler<ApplyTemplateCommand, Result<ApplyTemplateResultDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public ApplyTemplateCommandHandler(IApplicationDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<Result<ApplyTemplateResultDto>> Handle(
        ApplyTemplateCommand request, 
        CancellationToken cancellationToken)
    {
        // Validate restaurant
        var restaurant = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken);

        if (restaurant == null)
        {
            return Result<ApplyTemplateResultDto>.FailureResult("Restaurant not found");
        }

        // Validate template
        var template = await _context.MenuTemplates
            .FirstOrDefaultAsync(t => t.Id == request.TemplateId, cancellationToken);

        if (template == null)
        {
            return Result<ApplyTemplateResultDto>.FailureResult("Template not found");
        }

        // Generate menu from template
        var generateCommand = new GenerateMenuCommand(
            RestaurantId: request.RestaurantId,
            TemplateId: request.TemplateId,
            OverwriteExisting: request.OverwriteExisting,
            TemplateKey: null,
            TargetCategories: null,
            TargetItemsPerCategory: null,
            LanguageCodes: null
        );

        var generateResult = await _mediator.Send(generateCommand, cancellationToken);

        if (!generateResult.Success)
        {
            return Result<ApplyTemplateResultDto>.FailureResult(generateResult.Message);
        }

        // Update restaurant active template
        restaurant.ActiveTemplateId = request.TemplateId;
        restaurant.LastMenuUpdate = DateTime.UtcNow;

        // Update template usage stats
        template.UsageCount++;
        template.LastUsedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var result = new ApplyTemplateResultDto
        {
            RestaurantId = request.RestaurantId,
            TemplateId = request.TemplateId,
            CategoriesCreated = generateResult.Data!.CategoriesCreated,
            ItemsCreated = generateResult.Data.ItemsCreated,
            AppliedAt = DateTime.UtcNow
        };

        return Result<ApplyTemplateResultDto>.SuccessResult(result, "Template applied successfully");
    }
}
```

### 2. Update Restaurant Settings Command

**File:** `backend/src/Menufy.Application/Features/Restaurants/Commands/UpdateSettings/UpdateRestaurantSettingsCommand.cs` (NEW)

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

**Handler:** `UpdateRestaurantSettingsCommandHandler.cs`

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
        {
            return Result<RestaurantSettingsDto>.FailureResult("Restaurant not found");
        }

        // Update active template
        if (request.Settings.ActiveTemplateId.HasValue)
        {
            restaurant.ActiveTemplateId = request.Settings.ActiveTemplateId;
        }

        // Update custom theme
        if (request.Settings.CustomTheme != null)
        {
            restaurant.CustomTheme = JsonSerializer.Serialize(request.Settings.CustomTheme);
        }

        // Update display settings
        if (request.Settings.DisplaySettings != null)
        {
            restaurant.MenuDisplaySettings = JsonSerializer.Serialize(request.Settings.DisplaySettings);
        }

        // Update currency and language
        if (!string.IsNullOrWhiteSpace(request.Settings.Currency))
        {
            restaurant.Currency = request.Settings.Currency;
        }

        if (!string.IsNullOrWhiteSpace(request.Settings.DefaultLanguage))
        {
            restaurant.DefaultLanguage = request.Settings.DefaultLanguage;
        }

        restaurant.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        // Build response
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

### 3. Get Restaurant Settings Query

**File:** `backend/src/Menufy.Application/Features/Restaurants/Queries/GetSettings/GetRestaurantSettingsQuery.cs` (NEW)

```csharp
using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Restaurants.DTOs;

namespace Menufy.Application.Features.Restaurants.Queries.GetSettings;

public record GetRestaurantSettingsQuery(Guid RestaurantId) 
    : IRequest<Result<RestaurantSettingsDto>>;
```

**Handler:** `GetRestaurantSettingsQueryHandler.cs`

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
        {
            return Result<RestaurantSettingsDto>.FailureResult("Restaurant not found");
        }

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

### 4. Update Public Menu Query

**File:** `backend/src/Menufy.Application/Features/Menus/Queries/GetPublicMenu/GetPublicMenuQueryHandler.cs`

Update to include theme and settings:

```csharp
public async Task<Result<PublicMenuDto>> Handle(
    GetPublicMenuQuery request, 
    CancellationToken cancellationToken)
{
    var restaurant = await _context.Restaurants
        .AsNoTracking()
        .Include(r => r.ActiveTemplate) // NEW: Include template
        .FirstOrDefaultAsync(r => r.Slug == request.Slug, cancellationToken);

    if (restaurant == null)
    {
        return Result<PublicMenuDto>.FailureResult("Restaurant not found");
    }

    var categories = await _context.MenuCategories
        .Where(c => c.RestaurantId == restaurant.Id)
        .OrderBy(c => c.DisplayOrder)
        .Include(c => c.MenuItems)
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    var categoriesDto = MenuCategoryTreeBuilder.BuildTree(
        categories,
        request.Language,
        item => item.IsAvailable);

    var localizedRestaurantName = string.IsNullOrWhiteSpace(request.Language)
        ? restaurant.GetTranslatedName()
        : restaurant.GetTranslatedName(request.Language);

    // NEW: Load theme
    MenuTemplateThemeDto? theme = null;
    if (!string.IsNullOrWhiteSpace(restaurant.CustomTheme))
    {
        theme = JsonSerializer.Deserialize<MenuTemplateThemeDto>(restaurant.CustomTheme);
    }
    else if (restaurant.ActiveTemplate?.Theme != null)
    {
        theme = JsonSerializer.Deserialize<MenuTemplateThemeDto>(restaurant.ActiveTemplate.Theme);
    }

    // NEW: Load display settings
    var displaySettings = !string.IsNullOrWhiteSpace(restaurant.MenuDisplaySettings)
        ? JsonSerializer.Deserialize<MenuDisplaySettingsDto>(restaurant.MenuDisplaySettings)
        : new MenuDisplaySettingsDto();

    // NEW: Track view
    restaurant.TotalMenuViews++;
    await _context.SaveChangesAsync(cancellationToken);

    var menuDto = new PublicMenuDto
    {
        RestaurantId = restaurant.Id,
        RestaurantName = restaurant.Name,
        RestaurantLocalizedName = localizedRestaurantName,
        LogoUrl = restaurant.LogoUrl,
        ContactPhone = restaurant.ContactPhone,
        ContactEmail = restaurant.ContactEmail,
        Address = restaurant.Address,
        Language = request.Language ?? "en",
        Theme = theme, // NEW
        DisplaySettings = displaySettings, // NEW
        Currency = restaurant.Currency ?? "USD", // NEW
        Categories = categoriesDto
    };

    return Result<PublicMenuDto>.SuccessResult(menuDto);
}
```

---

## 🌐 API Endpoints

### New Restaurant Settings Controller

**File:** `backend/src/Menufy.API/Controllers/RestaurantSettingsController.cs` (NEW)

```csharp
using System.Security.Claims;
using MediatR;
using Menufy.Application.Features.Restaurants.Commands.ApplyTemplate;
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

    /// <summary>
    /// Get restaurant menu settings
    /// </summary>
    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings(Guid restaurantId)
    {
        var result = await _mediator.Send(new GetRestaurantSettingsQuery(restaurantId));

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Update restaurant menu settings
    /// </summary>
    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSettings(
        Guid restaurantId, 
        [FromBody] UpdateRestaurantSettingsDto dto)
    {
        var result = await _mediator.Send(
            new UpdateRestaurantSettingsCommand(restaurantId, dto));

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Apply a template to restaurant menu
    /// </summary>
    [HttpPost("apply-template")]
    public async Task<IActionResult> ApplyTemplate(
        Guid restaurantId,
        [FromBody] ApplyTemplateRequest request)
    {
        var command = new ApplyTemplateCommand(
            restaurantId,
            request.TemplateId,
            request.OverwriteExisting);

        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}

public record ApplyTemplateRequest(Guid TemplateId, bool OverwriteExisting = false);
```

### Complete API Endpoints Summary

```http
# Menu Templates
GET    /api/menu-templates                     # List all templates
GET    /api/menu-templates/{id}                # Get template by ID
POST   /api/menu-templates                     # Create template
PUT    /api/menu-templates/{id}                # Update template
DELETE /api/menu-templates/{id}                # Delete template

# Restaurant Settings (NEW)
GET    /api/restaurants/{id}/settings          # Get settings
PUT    /api/restaurants/{id}/settings          # Update settings
POST   /api/restaurants/{id}/apply-template    # Apply template

# Menu Generation
POST   /api/restaurants/{id}/generate-menu     # Generate menu

# Public Menu
GET    /api/menu/{slug}?lang=en                # Get public menu

# QR Codes
GET    /api/restaurants/{id}/qr-code           # Get QR code
POST   /api/restaurants/{id}/qr-code           # Generate QR code
```

---

## 📝 Implementation Steps

### Step 1: Update Domain Entities

```bash
# 1. Update MenuTemplate entity
# 2. Update Restaurant entity
# 3. Create migration
cd backend/src/Menufy.Infrastructure
dotnet ef migrations add EnhancedMenuGenerator
```

### Step 2: Update DTOs

```bash
# 1. Update MenuTemplateThemeDto
# 2. Create RestaurantSettingsDto
# 3. Update PublicMenuDto
```

### Step 3: Create New Commands/Queries

```bash
# 1. Create ApplyTemplateCommand
# 2. Create UpdateRestaurantSettingsCommand
# 3. Create GetRestaurantSettingsQuery
# 4. Update GetPublicMenuQueryHandler
```

### Step 4: Create Controller

```bash
# 1. Create RestaurantSettingsController
# 2. Add new endpoints
```

### Step 5: Apply Migration

```bash
cd backend/src/Menufy.Infrastructure
dotnet ef database update
```

### Step 6: Test Endpoints

```bash
# Test with Postman or Swagger
dotnet run --project src/Menufy.API
# Navigate to: https://localhost:5001/swagger
```

---

## 🧪 Testing Examples

### 1. Create Enhanced Template

```http
POST /api/menu-templates
Content-Type: application/json

{
  "name": "Modern Italian Restaurant",
  "description": "Elegant Italian dining experience",
  "isPublished": true,
  "tags": ["italian", "fine-dining"],
  "theme": {
    "primaryColor": "#dc2626",
    "accentColor": "#f59e0b",
    "backgroundColor": "#fafaf9",
    "surfaceColor": "#ffffff",
    "textColor": "#292524",
    "fontFamily": "Playfair Display",
    "fontSize": "large",
    "layout": "cards",
    "cardStyle": "modern",
    "borderRadius": "large",
    "spacing": "relaxed",
    "showImages": true,
    "imageSize": "large",
    "imageShape": "rounded",
    "logoPosition": "center",
    "showRestaurantInfo": true,
    "headerStyle": "banner"
  },
  "structure": {
    "categories": [
      {
        "name": "Antipasti",
        "translations": { "ar": "المقبلات", "it": "Antipasti" },
        "displayOrder": 1,
        "icon": "🍝",
        "items": [
          {
            "name": "Bruschetta",
            "nameTranslations": { "ar": "بروشيتا" },
            "description": "Toasted bread with tomatoes and basil",
            "descriptionTranslations": { "ar": "خبز محمص مع الطماطم والريحان" },
            "price": 8.50,
            "imageUrl": "https://example.com/bruschetta.jpg",
            "isAvailable": true,
            "displayOrder": 1,
            "tags": ["vegetarian", "popular"]
          }
        ]
      }
    ]
  }
}
```

### 2. Apply Template to Restaurant

```http
POST /api/restaurants/{restaurantId}/apply-template
Content-Type: application/json

{
  "templateId": "template-guid-here",
  "overwriteExisting": false
}
```

### 3. Update Restaurant Settings

```http
PUT /api/restaurants/{restaurantId}/settings
Content-Type: application/json

{
  "activeTemplateId": "template-guid-here",
  "customTheme": {
    "primaryColor": "#059669",
    "accentColor": "#10b981",
    "layout": "grid"
  },
  "displaySettings": {
    "showPrices": true,
    "showImages": true,
    "enableSearch": true,
    "availableLanguages": ["en", "ar", "fr"]
  },
  "currency": "EUR",
  "defaultLanguage": "en"
}
```

### 4. Get Public Menu with Theme

```http
GET /api/menu/my-restaurant?lang=en

Response:
{
  "success": true,
  "data": {
    "restaurantId": "...",
    "restaurantName": "My Restaurant",
    "theme": {
      "primaryColor": "#dc2626",
      "layout": "cards",
      ...
    },
    "displaySettings": {
      "showPrices": true,
      ...
    },
    "categories": [...]
  }
}
```

---

## 🔐 Security Considerations

### Authorization Rules

1. **Templates:**
   - Owners can create templates for their restaurants
   - Admins can create global templates
   - Users can only edit/delete their own templates

2. **Restaurant Settings:**
   - Only restaurant owner can update settings
   - Admins can update any restaurant

3. **Public Menu:**
   - No authentication required
   - Track views for analytics

### Implementation

```csharp
// In command handlers
var isAdmin = string.Equals(request.UserRole, "Admin", StringComparison.OrdinalIgnoreCase);

if (!isAdmin && restaurant.OwnerId != request.UserId)
{
    return Result.FailureResult("Unauthorized");
}
```

---

## 📊 Analytics & Tracking

### Track Template Usage

```csharp
// When template is applied
template.UsageCount++;
template.LastUsedAt = DateTime.UtcNow;
```

### Track Menu Views

```csharp
// In GetPublicMenuQueryHandler
restaurant.TotalMenuViews++;
await _context.SaveChangesAsync(cancellationToken);
```

### Future: Detailed Analytics

```csharp
public class MenuAnalytics : BaseEntity
{
    public Guid RestaurantId { get; set; }
    public DateTime ViewedAt { get; set; }
    public string? Language { get; set; }
    public string? DeviceType { get; set; }
    public string? UserAgent { get; set; }
    public TimeSpan TimeSpent { get; set; }
}
```

---

## 🚀 Deployment Checklist

- [ ] Update domain entities
- [ ] Create and apply migrations
- [ ] Update DTOs
- [ ] Create new commands/queries
- [ ] Create new controller
- [ ] Test all endpoints
- [ ] Update API documentation
- [ ] Deploy to staging
- [ ] Run integration tests
- [ ] Deploy to production

---

This guide provides everything needed to implement the enhanced menu generator on the backend! 🎉

