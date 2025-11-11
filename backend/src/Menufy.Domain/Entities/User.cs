using Menufy.Domain.Common;
using Menufy.Domain.Enums;

namespace Menufy.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.RestaurantOwner;

    // Navigation properties
    public ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
}
