using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.Auth.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
            return Result.FailureResult("User not found");

        if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            return Result.FailureResult("Current password is incorrect");

        user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.SuccessResult("Password changed successfully");
    }
}
