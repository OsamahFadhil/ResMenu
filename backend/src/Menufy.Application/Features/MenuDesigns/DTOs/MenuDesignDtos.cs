using Menufy.Application.Features.MenuTemplates.DTOs;
using Menufy.Domain.Enums;

namespace Menufy.Application.Features.MenuDesigns.DTOs;

/// <summary>
/// DTO for a single category's layout configuration
/// </summary>
public class CategoryLayoutDto
{
    public Guid CategoryId { get; set; }
    public int DisplayOrder { get; set; }
    public string Layout { get; set; } = "list"; // "list" | "grid" | "cards"
    public string CardStyle { get; set; } = "modern"; // "modern" | "classic" | "minimal"
    public int? Columns { get; set; } // For grid layout
    public string? Spacing { get; set; } // "compact" | "normal" | "relaxed"
    public string? BorderRadius { get; set; } // "none" | "small" | "medium" | "large"
    public string? ImageSize { get; set; } // "small" | "medium" | "large"
    public string? ImageShape { get; set; } // "square" | "rounded" | "circle"
    public bool ShowImages { get; set; } = true;
    public bool ShowPrices { get; set; } = true;
    public bool ShowDescriptions { get; set; } = true;
}

/// <summary>
/// DTO for the complete layout configuration
/// </summary>
public class LayoutConfigurationDto
{
    public List<CategoryLayoutDto> Categories { get; set; } = new();
    public GlobalLayoutSettingsDto GlobalSettings { get; set; } = new();
}

/// <summary>
/// Global layout settings that apply across the menu
/// </summary>
public class GlobalLayoutSettingsDto
{
    public string Spacing { get; set; } = "normal";
    public string BorderRadius { get; set; } = "medium";
    public string FontFamily { get; set; } = "Inter";
    public string HeaderStyle { get; set; } = "simple"; // "simple" | "banner" | "overlay"
    public string LogoPosition { get; set; } = "center"; // "left" | "center" | "right"
    public bool ShowRestaurantInfo { get; set; } = true;
    public bool ShowLogo { get; set; } = true;
    public string HeaderAlignment { get; set; } = "center"; // "left" | "center" | "right"
    public string? Tagline { get; set; }
}

/// <summary>
/// DTO for retrieving a menu design
/// </summary>
public class MenuDesignDto
{
    public Guid Id { get; set; }
    public Guid RestaurantId { get; set; }
    public LayoutConfigurationDto LayoutConfiguration { get; set; } = new();
    public MenuTemplateThemeDto GlobalTheme { get; set; } = new();
    public int Version { get; set; }
    public bool IsActive { get; set; }
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? HeaderImageUrl { get; set; }

    public string? HeaderColor { get; set; }

    public DisplayMode? HeaderDisplayMode { get; set; }
}

/// <summary>
/// DTO for creating or updating a menu design
/// </summary>
public class SaveMenuDesignDto
{
    public Guid RestaurantId { get; set; }
    public LayoutConfigurationDto LayoutConfiguration { get; set; } = new();
    public MenuTemplateThemeDto GlobalTheme { get; set; } = new();
    public string? Name { get; set; }
    public bool SetAsActive { get; set; } = true;
    public string? HeaderImageUrl { get; set; }

    public string? HeaderColor { get; set; }

    public DisplayMode? HeaderDisplayMode { get; set; } 
}

/// <summary>
/// DTO for the response after saving a design
/// </summary>
public class SaveMenuDesignResultDto
{
    public Guid Id { get; set; }
    public int Version { get; set; }
    public bool IsActive { get; set; }
    public string Message { get; set; } = string.Empty;
}

