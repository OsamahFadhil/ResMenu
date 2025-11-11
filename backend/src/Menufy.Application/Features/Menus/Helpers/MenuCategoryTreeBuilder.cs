using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Menufy.Application.Features.Menus.DTOs;
using Menufy.Domain.Entities;

namespace Menufy.Application.Features.Menus.Helpers;

public static class MenuCategoryTreeBuilder
{
    public static List<MenuCategoryDto> BuildTree(
        IEnumerable<MenuCategory> categories,
        string? language = null,
        Func<MenuItem, bool>? itemFilter = null)
    {
        itemFilter ??= _ => true;
        var normalizedLanguage = NormalizeLanguage(language);

        var lookup = categories.ToDictionary(
            c => c.Id,
            c => MapCategory(c, normalizedLanguage, itemFilter));

        var roots = new List<MenuCategoryDto>();

        foreach (var category in categories.OrderBy(c => c.DisplayOrder))
        {
            var dto = lookup[category.Id];

            if (category.ParentCategoryId.HasValue && lookup.TryGetValue(category.ParentCategoryId.Value, out var parentDto))
            {
                InsertSorted(parentDto.Children, dto);
            }
            else
            {
                InsertSorted(roots, dto);
            }
        }

        return roots;
    }

    private static MenuCategoryDto MapCategory(
        MenuCategory category,
        string? language,
        Func<MenuItem, bool> itemFilter)
    {
        var translations = DeserializeCategoryTranslations(category.Translations);
        var localizedName = ResolveLocalizedCategoryName(category, language);

        return new MenuCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            LocalizedName = localizedName,
            DisplayOrder = category.DisplayOrder,
            ParentCategoryId = category.ParentCategoryId,
            Translations = translations,
            Items = category.MenuItems
                .Where(itemFilter)
                .OrderBy(i => i.DisplayOrder)
                .Select(i => MapItem(i, language))
                .ToList(),
            Children = new List<MenuCategoryDto>()
        };
    }

    private static MenuItemDto MapItem(MenuItem item, string? language)
    {
        var translations = DeserializeMenuItemTranslations(item.Translations);
        var localizedName = ResolveLocalizedMenuItemName(item, language);
        var localizedDescription = ResolveLocalizedMenuItemDescription(item, language);

        return new MenuItemDto
        {
            Id = item.Id,
            Name = item.Name,
            LocalizedName = localizedName,
            Description = item.Description,
            LocalizedDescription = localizedDescription,
            Price = item.Price,
            ImageUrl = item.ImageUrl,
            IsAvailable = item.IsAvailable,
            DisplayOrder = item.DisplayOrder,
            Translations = translations
        };
    }

    private static void InsertSorted(List<MenuCategoryDto> target, MenuCategoryDto dto)
    {
        var index = target.FindIndex(c => c.DisplayOrder > dto.DisplayOrder);
        if (index >= 0)
        {
            target.Insert(index, dto);
        }
        else
        {
            target.Add(dto);
        }
    }

    private static Dictionary<string, string>? DeserializeCategoryTranslations(string? translations)
    {
        if (string.IsNullOrWhiteSpace(translations))
        {
            return null;
        }

        try
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(translations);
            return data?.Count > 0 ? data : null;
        }
        catch
        {
            return null;
        }
    }

    private static Dictionary<string, MenuItemTranslationDto>? DeserializeMenuItemTranslations(string? translations)
    {
        if (string.IsNullOrWhiteSpace(translations))
        {
            return null;
        }

        try
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(translations);
            if (data == null || data.Count == 0)
            {
                return null;
            }

            return data.ToDictionary(
                kvp => kvp.Key,
                kvp => new MenuItemTranslationDto
                {
                    Name = kvp.Value.TryGetValue("name", out var name) ? name : null,
                    Description = kvp.Value.TryGetValue("description", out var description) ? description : null
                });
        }
        catch
        {
            return null;
        }
    }

    private static string ResolveLocalizedCategoryName(MenuCategory category, string? language)
    {
        return string.IsNullOrWhiteSpace(language)
            ? category.GetTranslatedName()
            : category.GetTranslatedName(language);
    }

    private static string ResolveLocalizedMenuItemName(MenuItem item, string? language)
    {
        return string.IsNullOrWhiteSpace(language)
            ? item.GetTranslatedName()
            : item.GetTranslatedName(language);
    }

    private static string? ResolveLocalizedMenuItemDescription(MenuItem item, string? language)
    {
        return string.IsNullOrWhiteSpace(language)
            ? item.GetTranslatedDescription()
            : item.GetTranslatedDescription(language);
    }

    private static string? NormalizeLanguage(string? language)
    {
        return string.IsNullOrWhiteSpace(language)
            ? null
            : language.Trim().ToLowerInvariant();
    }
}

