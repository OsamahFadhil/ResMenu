using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Menufy.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public ForgotPasswordCommandHandler(IApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        // Don't reveal if user exists or not for security
        if (user == null)
            return Result.SuccessResult("If the email exists, a password reset link has been sent.");

        // Generate reset token
        var resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        user.ResetPasswordToken = resetToken;
        user.ResetPasswordExpiry = DateTime.UtcNow.AddHours(1);

        await _context.SaveChangesAsync(cancellationToken);

        // Send email
        await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken, cancellationToken);

        return Result.SuccessResult("If the email exists, a password reset link has been sent.");
    }
}
