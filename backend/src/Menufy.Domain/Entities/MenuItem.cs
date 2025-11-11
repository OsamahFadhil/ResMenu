using Menufy.Domain.Common;

namespace Menufy.Domain.Entities;

public class MenuItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int DisplayOrder { get; set; }
    public Guid CategoryId { get; set; }

    // Navigation properties
    public MenuCategory Category { get; set; } = null!;
}
