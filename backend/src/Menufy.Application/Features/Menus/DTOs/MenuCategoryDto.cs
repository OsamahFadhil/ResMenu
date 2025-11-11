using System;
using System.Collections.Generic;

namespace Menufy.Application.Features.Menus.DTOs;

public class MenuCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LocalizedName { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public Dictionary<string, string>? Translations { get; set; }
    public List<MenuCategoryDto> Children { get; set; } = new();
    public List<MenuItemDto> Items { get; set; } = new();
}

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public Dictionary<string, string>? Translations { get; set; }
}

public class UpdateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public Dictionary<string, string>? Translations { get; set; }
}
