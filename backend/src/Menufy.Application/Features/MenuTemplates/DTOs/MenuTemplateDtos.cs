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
    public string PrimaryColor { get; set; } = "#f97316";
    public string AccentColor { get; set; } = "#facc15";
    public string BackgroundColor { get; set; } = "#fff7ed";
    public string SurfaceColor { get; set; } = "#ffffff";
    public string TextColor { get; set; } = "#1f2937";
    public string FontFamily { get; set; } = "Inter";
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

