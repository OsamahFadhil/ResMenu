using System;
using System.Collections.Generic;
using Menufy.Application.Features.MenuTemplates.DTOs;
using Menufy.Application.Features.Restaurants.DTOs;
using Menufy.Application.Features.MenuDesigns.DTOs;
using Menufy.Domain.Enums;

namespace Menufy.Application.Features.Menus.DTOs;

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
    
    // Theme & Settings (Legacy - for backward compatibility)
    public MenuTemplateThemeDto? Theme { get; set; }
    public MenuDisplaySettingsDto? DisplaySettings { get; set; }
    public string Currency { get; set; } = "USD";
    
    // New Design System
    public LayoutConfigurationDto? LayoutConfiguration { get; set; }
    
    // Header settings from MenuDesign
    public string? HeaderColor { get; set; }
    public string? HeaderImageUrl { get; set; }
    public DisplayMode? HeaderDisplayMode { get; set; }
    
    public List<MenuCategoryDto> Categories { get; set; } = new();
}
