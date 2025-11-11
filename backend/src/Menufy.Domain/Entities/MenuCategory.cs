using Menufy.Domain.Common;

namespace Menufy.Domain.Entities;

public class MenuCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid? ParentCategoryId { get; set; }

    // Navigation properties
    public Restaurant Restaurant { get; set; } = null!;
    public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    public MenuCategory? ParentCategory { get; set; }
    public ICollection<MenuCategory> Children { get; set; } = new List<MenuCategory>();
}
