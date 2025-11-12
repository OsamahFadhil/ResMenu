using Menufy.Application.Features.MenuTemplates.DTOs;

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

