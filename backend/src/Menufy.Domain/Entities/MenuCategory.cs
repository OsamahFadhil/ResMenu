using Menufy.Domain.Common;
using System.Text.Json;

namespace Menufy.Domain.Entities;

public class MenuCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Translations { get; set; } // JSON: {"en": "Drinks", "ar": "مشروبات"}
    public int DisplayOrder { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid? ParentCategoryId { get; set; }

    // Navigation properties
    public Restaurant Restaurant { get; set; } = null!;
    public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    public MenuCategory? ParentCategory { get; set; }
    public ICollection<MenuCategory> Children { get; set; } = new List<MenuCategory>();

    // Helper method to get translated name
    public string GetTranslatedName(string language = "en")
    {
        if (string.IsNullOrEmpty(Translations))
            return Name;

        try
        {
            var translations = JsonSerializer.Deserialize<Dictionary<string, string>>(Translations);
            return translations != null && translations.ContainsKey(language)
                ? translations[language]
                : Name;
        }
        catch
        {
            return Name;
        }
    }
}
