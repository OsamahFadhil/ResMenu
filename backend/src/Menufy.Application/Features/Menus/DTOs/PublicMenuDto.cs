using System;
using System.Collections.Generic;
using Menufy.Application.Features.MenuTemplates.DTOs;
using Menufy.Application.Features.Restaurants.DTOs;
using Menufy.Application.Features.MenuDesigns.DTOs;

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
    
    public List<MenuCategoryDto> Categories { get; set; } = new();
}
