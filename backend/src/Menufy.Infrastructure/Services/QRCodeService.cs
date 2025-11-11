using Menufy.Application.Common.Interfaces;
using QRCoder;

namespace Menufy.Infrastructure.Services;

public class QRCodeService : IQRCodeService
{
    public Task<string> GenerateQRCodeAsync(string content, CancellationToken cancellationToken = default)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);

        var qrCodeBytes = qrCode.GetGraphic(20);
        var base64String = Convert.ToBase64String(qrCodeBytes);
        var dataUrl = $"data:image/png;base64,{base64String}";

        return Task.FromResult(dataUrl);
    }
}
