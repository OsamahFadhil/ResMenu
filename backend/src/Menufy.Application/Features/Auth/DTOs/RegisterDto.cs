namespace Menufy.Application.Features.Auth.DTOs;

public class RegisterDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string RestaurantName { get; set; } = string.Empty;
    public string? ContactPhone { get; set; }
}
