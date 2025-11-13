using Menufy.Domain.Common;

namespace Menufy.Domain.Entities;

public class MenuTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? RestaurantId { get; set; }
    public string Structure { get; set; } = string.Empty;
    public string? Theme { get; set; }
    public string? Tags { get; set; }
    public bool IsPublished { get; set; }

    // Usage Tracking
    public int UsageCount { get; set; } = 0;
    public DateTime? LastUsedAt { get; set; }

    // Design-Only Template Support (New Designer System)
    /// <summary>
    /// If true, this template only contains design/theme settings, no content (categories/items)
    /// Used by the new visual designer system
    /// </summary>
    public bool IsDesignOnly { get; set; } = false;
    
    /// <summary>
    /// JSON configuration for layout settings when IsDesignOnly = true
    /// Structure matches MenuDesign.LayoutConfiguration
    /// </summary>
    public string? LayoutConfiguration { get; set; }

    public Restaurant? Restaurant { get; set; }
}

