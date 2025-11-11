using Menufy.Domain.Common;

namespace Menufy.Domain.Entities;

public class QRCode : BaseEntity
{
    public string ImageUrl { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public Guid RestaurantId { get; set; }

    // Navigation properties
    public Restaurant Restaurant { get; set; } = null!;
}
