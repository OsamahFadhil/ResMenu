namespace Menufy.Application.Features.Menus.DTOs;

public class PublicMenuDto
{
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? Address { get; set; }
    public List<MenuCategoryDto> Categories { get; set; } = new();
}
