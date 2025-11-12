using System.Text.Json;
using System.Text.Json.Serialization;

namespace Menufy.Application.Features.MenuTemplates.DTOs;

public class MenuTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? RestaurantId { get; set; }
    public bool IsGlobal => !RestaurantId.HasValue;
    public bool IsPublished { get; set; }
    public List<string> Tags { get; set; } = new();
    public MenuTemplateThemeDto? Theme { get; set; }
    public MenuTemplateStructureDto Structure { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class UpsertMenuTemplateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? RestaurantId { get; set; }
    public bool IsPublished { get; set; }
    public List<string>? Tags { get; set; }
    public MenuTemplateThemeDto? Theme { get; set; }
    public MenuTemplateStructureDto Structure { get; set; } = new();
}

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

public class MenuTemplateStructureDto
{
    public List<MenuTemplateCategoryDto> Categories { get; set; } = new();
}

public class MenuTemplateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string>? Translations { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public List<MenuTemplateItemDto> Items { get; set; } = new();
}

public class MenuTemplateItemDto
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string>? NameTranslations { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, string>? DescriptionTranslations { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int DisplayOrder { get; set; }
    public List<string>? Tags { get; set; }
    public decimal? Rating { get; set; }
}

internal static class MenuTemplateSerializationOptions
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}

