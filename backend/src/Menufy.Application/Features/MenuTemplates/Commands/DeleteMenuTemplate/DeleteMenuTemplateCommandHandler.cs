using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.MenuTemplates.Commands.DeleteMenuTemplate;

public class DeleteMenuTemplateCommandHandler : IRequestHandler<DeleteMenuTemplateCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteMenuTemplateCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteMenuTemplateCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.MenuTemplates
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            return Result.FailureResult("Template not found");
        }

        var isAdmin = string.Equals(request.UserRole, "Admin", StringComparison.OrdinalIgnoreCase);

        if (!isAdmin)
        {
            var restaurant = await _context.Restaurants
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == entity.RestaurantId, cancellationToken);

            if (restaurant == null || restaurant.OwnerId != request.UserId)
            {
                return Result.FailureResult("You are not allowed to delete this template");
            }
        }

        _context.MenuTemplates.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.SuccessResult("Template deleted");
    }
}

