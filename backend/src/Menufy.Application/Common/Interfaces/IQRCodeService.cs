namespace Menufy.Application.Common.Interfaces;

public interface IQRCodeService
{
    Task<string> GenerateQRCodeAsync(string content, CancellationToken cancellationToken = default);
}
