namespace Menufy.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default);
    Task SendPasswordResetEmailAsync(string to, string resetToken, CancellationToken cancellationToken = default);
    Task SendWelcomeEmailAsync(string to, string name, CancellationToken cancellationToken = default);
}
