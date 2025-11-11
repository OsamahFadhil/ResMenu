using Menufy.Domain.Common;
using Menufy.Domain.Enums;

namespace Menufy.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.RestaurantOwner;
    public bool IsActive { get; set; } = true;
    public bool EmailVerified { get; set; } = false;
    public string? ResetPasswordToken { get; set; }
    public DateTime? ResetPasswordExpiry { get; set; }

    // Navigation properties
    public ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
