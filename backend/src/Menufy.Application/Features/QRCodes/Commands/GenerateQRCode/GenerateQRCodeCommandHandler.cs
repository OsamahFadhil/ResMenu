using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.QRCodes.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.QRCodes.Commands.GenerateQRCode;

public class GenerateQRCodeCommandHandler : IRequestHandler<GenerateQRCodeCommand, Result<QRCodeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IQRCodeService _qrCodeService;

    public GenerateQRCodeCommandHandler(
        IApplicationDbContext context,
        IQRCodeService qrCodeService)
    {
        _context = context;
        _qrCodeService = qrCodeService;
    }

    public async Task<Result<QRCodeDto>> Handle(GenerateQRCodeCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await _context.Restaurants
            .Include(r => r.QRCode)
            .FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken);

        if (restaurant == null)
        {
            return Result<QRCodeDto>.FailureResult("Restaurant not found");
        }

        var menuLink = $"{request.BaseUrl}/menu/{restaurant.Slug}";

        // Check if QR code already exists
        if (restaurant.QRCode != null)
        {
            var existingDto = new QRCodeDto
            {
                Id = restaurant.QRCode.Id,
                ImageUrl = restaurant.QRCode.ImageUrl,
                Link = restaurant.QRCode.Link,
                RestaurantId = restaurant.QRCode.RestaurantId
            };

            return Result<QRCodeDto>.SuccessResult(existingDto, "QR Code already exists");
        }

        // Generate new QR code
        var qrCodeImageUrl = await _qrCodeService.GenerateQRCodeAsync(menuLink, cancellationToken);

        var qrCode = new Domain.Entities.QRCode
        {
            Id = Guid.NewGuid(),
            ImageUrl = qrCodeImageUrl,
            Link = menuLink,
            RestaurantId = request.RestaurantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.QRCodes.Add(qrCode);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new QRCodeDto
        {
            Id = qrCode.Id,
            ImageUrl = qrCode.ImageUrl,
            Link = qrCode.Link,
            RestaurantId = qrCode.RestaurantId
        };

        return Result<QRCodeDto>.SuccessResult(dto, "QR Code generated successfully");
    }
}
