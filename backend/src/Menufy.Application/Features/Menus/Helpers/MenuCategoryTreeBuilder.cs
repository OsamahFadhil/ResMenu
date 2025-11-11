using System;
using System.Collections.Generic;
using System.Linq;
using Menufy.Application.Features.Menus.DTOs;
using Menufy.Domain.Entities;

namespace Menufy.Application.Features.Menus.Helpers;

public static class MenuCategoryTreeBuilder
{
    public static List<MenuCategoryDto> BuildTree(IEnumerable<MenuCategory> categories, Func<MenuItem, bool>? itemFilter = null)
    {
        itemFilter ??= _ => true;

        var lookup = categories.ToDictionary(
            c => c.Id,
            c => new MenuCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                DisplayOrder = c.DisplayOrder,
                ParentCategoryId = c.ParentCategoryId,
                Items = c.MenuItems
                    .Where(itemFilter)
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => new MenuItemDto
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Description = i.Description,
                        Price = i.Price,
                        ImageUrl = i.ImageUrl,
                        IsAvailable = i.IsAvailable,
                        DisplayOrder = i.DisplayOrder
                    })
                    .ToList(),
                Children = new List<MenuCategoryDto>()
            });

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
}

