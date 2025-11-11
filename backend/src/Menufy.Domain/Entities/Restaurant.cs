using Menufy.Domain.Common;
using System.Text.Json;

namespace Menufy.Domain.Entities;

public class Restaurant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? Address { get; set; }
    public string? Translations { get; set; } // JSON: {"en": {"name": "...", "address": "..."}, "ar": {...}}
    public Guid OwnerId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public User Owner { get; set; } = null!;
    public ICollection<MenuCategory> MenuCategories { get; set; } = new List<MenuCategory>();
    public QRCode? QRCode { get; set; }
    public ICollection<MenuTemplate> MenuTemplates { get; set; } = new List<MenuTemplate>();

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
}
