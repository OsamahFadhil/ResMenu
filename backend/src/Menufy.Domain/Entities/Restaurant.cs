using Menufy.Domain.Common;

namespace Menufy.Domain.Entities;

public class Restaurant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? Address { get; set; }
    public Guid OwnerId { get; set; }

    // Navigation properties
    public User Owner { get; set; } = null!;
    public ICollection<MenuCategory> MenuCategories { get; set; } = new List<MenuCategory>();
    public QRCode? QRCode { get; set; }
}
