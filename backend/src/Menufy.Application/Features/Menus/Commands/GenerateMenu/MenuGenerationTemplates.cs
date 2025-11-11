using Menufy.Application.Features.Menus.DTOs;
using Menufy.Application.Features.MenuTemplates.DTOs;
using System;
using System.Collections.Generic;

namespace Menufy.Application.Features.Menus.Commands.GenerateMenu;

internal static class MenuGenerationTemplates
{
    private static readonly MenuTemplate DefaultTemplate = BuildDefaultTemplate();

    public static MenuTemplate? ResolveTemplate(string? templateKey)
    {
        return string.IsNullOrWhiteSpace(templateKey) || templateKey.Equals("default", StringComparison.OrdinalIgnoreCase)
            ? DefaultTemplate
            : null;
    }

    public static MenuTemplateStructureDto ToStructure(MenuTemplate template)
    {
        return new MenuTemplateStructureDto
        {
            Categories = template.Categories.Select(category => new MenuTemplateCategoryDto
            {
                Name = category.Name,
                Translations = category.Translations,
                DisplayOrder = category.DisplayOrder ?? 0,
                Items = category.Items.Select(item => new MenuTemplateItemDto
                {
                    Name = item.Name,
                    NameTranslations = item.Translations?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Name ?? item.Name),
                    Description = item.Description,
                    DescriptionTranslations = item.Translations?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Description ?? item.Description ?? string.Empty),
                    Price = item.Price,
                    ImageUrl = item.ImageUrl,
                    IsAvailable = item.IsAvailable,
                    DisplayOrder = item.DisplayOrder ?? 0,
                    Tags = new List<string>(),
                }).ToList()
            }).ToList()
        };
    }

    private static MenuTemplate BuildDefaultTemplate()
    {
        return new MenuTemplate
        {
            Categories = new List<MenuCategoryTemplate>
            {
                new()
                {
                    Name = "Starters",
                    Translations = new Dictionary<string, string>
                    {
                        ["en"] = "Starters",
                        ["ar"] = "المقبلات"
                    },
                    Items = new List<MenuItemTemplate>
                    {
                        new()
                        {
                            Name = "Garlic Bread",
                            Description = "Toasted bread with garlic butter and herbs",
                            Price = 4.50m,
                            Translations = new Dictionary<string, MenuItemTranslationDto>
                            {
                                ["en"] = new MenuItemTranslationDto { Name = "Garlic Bread", Description = "Toasted bread with garlic butter and herbs" },
                                ["ar"] = new MenuItemTranslationDto { Name = "خبز بالثوم", Description = "خبز محمص مع زبدة الثوم والأعشاب" }
                            }
                        },
                        new()
                        {
                            Name = "Seasonal Soup",
                            Description = "Chef's choice soup made fresh daily",
                            Price = 6.00m,
                            Translations = new Dictionary<string, MenuItemTranslationDto>
                            {
                                ["en"] = new MenuItemTranslationDto { Name = "Seasonal Soup", Description = "Chef's choice soup made fresh daily" },
                                ["ar"] = new MenuItemTranslationDto { Name = "شوربة الموسم", Description = "شوربة من اختيار الشيف تُحضّر يومياً" }
                            }
                        }
                    }
                },
                new()
                {
                    Name = "Main Courses",
                    Translations = new Dictionary<string, string>
                    {
                        ["en"] = "Main Courses",
                        ["ar"] = "الأطباق الرئيسية"
                    },
                    Items = new List<MenuItemTemplate>
                    {
                        new()
                        {
                            Name = "Grilled Salmon",
                            Description = "Fresh Atlantic salmon with lemon butter sauce",
                            Price = 18.50m,
                            Translations = new Dictionary<string, MenuItemTranslationDto>
                            {
                                ["en"] = new MenuItemTranslationDto { Name = "Grilled Salmon", Description = "Fresh Atlantic salmon with lemon butter sauce" },
                                ["ar"] = new MenuItemTranslationDto { Name = "سلمون مشوي", Description = "سلمون أطلنطي طازج مع صلصة الليمون والزبدة" }
                            }
                        },
                        new()
                        {
                            Name = "Herb Chicken",
                            Description = "Roasted chicken breast with seasonal vegetables",
                            Price = 15.00m,
                            Translations = new Dictionary<string, MenuItemTranslationDto>
                            {
                                ["en"] = new MenuItemTranslationDto { Name = "Herb Chicken", Description = "Roasted chicken breast with seasonal vegetables" },
                                ["ar"] = new MenuItemTranslationDto { Name = "دجاج بالأعشاب", Description = "صدر دجاج مشوي مع خضروات موسمية" }
                            }
                        }
                    }
                },
                new()
                {
                    Name = "Desserts",
                    Translations = new Dictionary<string, string>
                    {
                        ["en"] = "Desserts",
                        ["ar"] = "الحلويات"
                    },
                    Items = new List<MenuItemTemplate>
                    {
                        new()
                        {
                            Name = "Chocolate Lava Cake",
                            Description = "Warm chocolate cake with molten center",
                            Price = 7.00m,
                            Translations = new Dictionary<string, MenuItemTranslationDto>
                            {
                                ["en"] = new MenuItemTranslationDto { Name = "Chocolate Lava Cake", Description = "Warm chocolate cake with molten center" },
                                ["ar"] = new MenuItemTranslationDto { Name = "كيكة الشوكولاتة السائلة", Description = "كيكة شوكولاتة دافئة بحشوة سائلة" }
                            }
                        },
                        new()
                        {
                            Name = "Classic Cheesecake",
                            Description = "Creamy cheesecake with berry compote",
                            Price = 6.50m,
                            Translations = new Dictionary<string, MenuItemTranslationDto>
                            {
                                ["en"] = new MenuItemTranslationDto { Name = "Classic Cheesecake", Description = "Creamy cheesecake with berry compote" },
                                ["ar"] = new MenuItemTranslationDto { Name = "تشيز كيك كلاسيكي", Description = "تشيز كيك كريمي مع صلصة التوت" }
                            }
                        }
                    }
                },
                new()
                {
                    Name = "Beverages",
                    Translations = new Dictionary<string, string>
                    {
                        ["en"] = "Beverages",
                        ["ar"] = "المشروبات"
                    },
                    Items = new List<MenuItemTemplate>
                    {
                        new()
                        {
                            Name = "Fresh Lemonade",
                            Description = "House-made lemonade with mint",
                            Price = 4.00m,
                            Translations = new Dictionary<string, MenuItemTranslationDto>
                            {
                                ["en"] = new MenuItemTranslationDto { Name = "Fresh Lemonade", Description = "House-made lemonade with mint" },
                                ["ar"] = new MenuItemTranslationDto { Name = "ليموناضة طازجة", Description = "ليموناضة منزلية مع النعناع" }
                            }
                        },
                        new()
                        {
                            Name = "Turkish Coffee",
                            Description = "Strong and aromatic coffee served the traditional way",
                            Price = 3.50m,
                            Translations = new Dictionary<string, MenuItemTranslationDto>
                            {
                                ["en"] = new MenuItemTranslationDto { Name = "Turkish Coffee", Description = "Strong and aromatic coffee served the traditional way" },
                                ["ar"] = new MenuItemTranslationDto { Name = "قهوة تركية", Description = "قهوة قوية وعطرية تقدم بالطريقة التقليدية" }
                            }
                        }
                    }
                }
            }
        };
    }

    internal class MenuTemplate
    {
        public List<MenuCategoryTemplate> Categories { get; set; } = new();
    }

    internal class MenuCategoryTemplate
    {
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, string>? Translations { get; set; }
        public int? DisplayOrder { get; set; }
        public List<MenuItemTemplate> Items { get; set; } = new();
    }

    internal class MenuItemTemplate
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; } = true;
        public Dictionary<string, MenuItemTranslationDto>? Translations { get; set; }
        public int? DisplayOrder { get; set; }
        public string? ImageUrl { get; set; }
    }
}

