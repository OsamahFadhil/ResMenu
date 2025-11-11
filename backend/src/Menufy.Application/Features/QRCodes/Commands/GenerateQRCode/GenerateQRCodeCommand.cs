using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.QRCodes.DTOs;

namespace Menufy.Application.Features.QRCodes.Commands.GenerateQRCode;

public record GenerateQRCodeCommand(Guid RestaurantId, string BaseUrl) : IRequest<Result<QRCodeDto>>;
