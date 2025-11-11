namespace Menufy.Application.Features.QRCodes.DTOs;

public class QRCodeDto
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public Guid RestaurantId { get; set; }
}
