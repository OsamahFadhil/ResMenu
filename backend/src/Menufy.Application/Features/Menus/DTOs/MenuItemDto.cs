using System;
using System.Collections.Generic;

namespace Menufy.Application.Features.Menus.DTOs;

public class MenuItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LocalizedName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LocalizedDescription { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public int DisplayOrder { get; set; }
    public Dictionary<string, MenuItemTranslationDto>? Translations { get; set; }
}

public class CreateMenuItemDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int DisplayOrder { get; set; }
    public Dictionary<string, MenuItemTranslationDto>? Translations { get; set; }
}

public class UpdateMenuItemDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public int DisplayOrder { get; set; }
    public Dictionary<string, MenuItemTranslationDto>? Translations { get; set; }
}

public class MenuItemTranslationDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}
