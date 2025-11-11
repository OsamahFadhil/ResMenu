using System;

namespace Menufy.Application.Features.Menus.DTOs;

public class MenuGenerationResultDto
{
    public Guid RestaurantId { get; set; }
    public int CategoriesCreated { get; set; }
    public int ItemsCreated { get; set; }
    public bool OverwroteExisting { get; set; }
    public Guid? TemplateId { get; set; }
    public string TemplateKey { get; set; } = "default";
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

