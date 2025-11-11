using Menufy.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Menufy.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual email sending logic (SMTP, SendGrid, etc.)
        _logger.LogInformation("Email sent to {To} with subject: {Subject}", to, subject);
        return Task.CompletedTask;
    }

    public async Task SendPasswordResetEmailAsync(string to, string resetToken, CancellationToken cancellationToken = default)
    {
        var subject = "Reset Your Password - Menufy Pro+";
        var body = $@"
            <h2>Reset Your Password</h2>
            <p>Click the link below to reset your password:</p>
            <p><a href='http://localhost:3000/reset-password?token={resetToken}'>Reset Password</a></p>
            <p>This link will expire in 1 hour.</p>
            <p>If you didn't request this, please ignore this email.</p>
        ";

        await SendEmailAsync(to, subject, body, true, cancellationToken);
    }

    public async Task SendWelcomeEmailAsync(string to, string name, CancellationToken cancellationToken = default)
    {
        var subject = "Welcome to Menufy Pro+!";
        var body = $@"
            <h2>Welcome {name}!</h2>
            <p>Thank you for joining Menufy Pro+. We're excited to have you on board!</p>
            <p>You can now start creating your digital menu and sharing it with your customers.</p>
            <p>If you have any questions, feel free to contact our support team.</p>
        ";

        await SendEmailAsync(to, subject, body, true, cancellationToken);
    }
}
