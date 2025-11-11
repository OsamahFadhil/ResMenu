using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.ResetPasswordToken == request.Token, cancellationToken);

        if (user == null)
            return Result.FailureResult("Invalid or expired reset token");

        if (user.ResetPasswordExpiry == null || user.ResetPasswordExpiry < DateTime.UtcNow)
            return Result.FailureResult("Reset token has expired");

        user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        user.ResetPasswordToken = null;
        user.ResetPasswordExpiry = null;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.SuccessResult("Password has been reset successfully");
    }
}
