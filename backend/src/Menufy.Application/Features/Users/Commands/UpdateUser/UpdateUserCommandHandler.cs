using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Common.Interfaces;
using Menufy.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
            return Result.FailureResult("User not found");

        // Check if email is already taken by another user
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email && u.Id != request.UserId, cancellationToken);

        if (emailExists)
            return Result.FailureResult("Email is already in use");

        user.Name = request.Name;
        user.Email = request.Email;

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            if (Enum.TryParse<UserRole>(request.Role, true, out var role))
            {
                user.Role = role;
            }
        }

        if (request.IsActive.HasValue)
        {
            user.IsActive = request.IsActive.Value;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.SuccessResult("User updated successfully");
    }
}
