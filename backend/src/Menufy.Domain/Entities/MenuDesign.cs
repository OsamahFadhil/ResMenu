using Menufy.Domain.Common;

namespace Menufy.Domain.Entities;

public class MenuDesign : BaseEntity
{
    public Guid RestaurantId { get; set; }
    
    /// <summary>
    /// JSON configuration for how categories are laid out and styled
    /// Structure: { "categories": [ { "categoryId": "guid", "displayOrder": 1, "layout": "grid", ... } ], "globalSettings": {...} }
    /// </summary>
    public string LayoutConfiguration { get; set; } = string.Empty;
    
    /// <summary>
    /// JSON for global theme settings (colors, fonts, etc.)
    /// Structure: MenuTemplateThemeDto
    /// </summary>
    public string GlobalTheme { get; set; } = string.Empty;
    
    /// <summary>
    /// Version number for tracking design changes
    /// </summary>
    public int Version { get; set; } = 1;
    
    /// <summary>
    /// Whether this is the active design being displayed on public menu
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Optional name for saved designs (e.g., "Summer Menu 2024")
    /// </summary>
    public string? Name { get; set; }
    
    // Navigation properties
    public Restaurant Restaurant { get; set; } = null!;
}

