using Menufy.Domain.Common;
using System.Text.Json;

namespace Menufy.Domain.Entities;

public class MenuItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Translations { get; set; } // JSON: {"en": {"name": "Coffee", "description": "..."}, "ar": {...}}
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int DisplayOrder { get; set; }
    public Guid CategoryId { get; set; }

    // Navigation properties
    public MenuCategory Category { get; set; } = null!;

    // Helper methods to get translated content
    public string GetTranslatedName(string language = "en")
    {
        if (string.IsNullOrEmpty(Translations))
            return Name;

        try
        {
            var translations = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(Translations);
            return translations != null && translations.ContainsKey(language) && translations[language].ContainsKey("name")
                ? translations[language]["name"]
                : Name;
        }
        catch
        {
            return Name;
        }
    }

    public string? GetTranslatedDescription(string language = "en")
    {
        if (string.IsNullOrEmpty(Translations))
            return Description;

        try
        {
            var translations = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(Translations);
            return translations != null && translations.ContainsKey(language) && translations[language].ContainsKey("description")
                ? translations[language]["description"]
                : Description;
        }
        catch
        {
            return Description;
        }
    }
}
