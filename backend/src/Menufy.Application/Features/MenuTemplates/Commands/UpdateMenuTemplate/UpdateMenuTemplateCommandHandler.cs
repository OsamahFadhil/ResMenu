using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.MenuTemplates.DTOs;
using Menufy.Application.Features.MenuTemplates.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.MenuTemplates.Commands.UpdateMenuTemplate;

public class UpdateMenuTemplateCommandHandler : IRequestHandler<UpdateMenuTemplateCommand, Result<MenuTemplateDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateMenuTemplateCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<MenuTemplateDto>> Handle(UpdateMenuTemplateCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.MenuTemplates
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            return Result<MenuTemplateDto>.FailureResult("Template not found");
        }

        var isAdmin = string.Equals(request.UserRole, "Admin", StringComparison.OrdinalIgnoreCase);

        if (!isAdmin)
        {
            var restaurant = await _context.Restaurants
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == entity.RestaurantId, cancellationToken);

            if (restaurant == null || restaurant.OwnerId != request.UserId)
            {
                return Result<MenuTemplateDto>.FailureResult("You are not allowed to modify this template");
            }
        }

        if (request.Template.RestaurantId.HasValue)
        {
            if (!isAdmin)
            {
                var restaurant = await _context.Restaurants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == request.Template.RestaurantId.Value, cancellationToken);

                if (restaurant == null || restaurant.OwnerId != request.UserId)
                {
                    return Result<MenuTemplateDto>.FailureResult("You are not allowed to assign this restaurant");
                }
            }
        }
        else
        {
            if (!isAdmin)
            {
                var ownedRestaurant = await _context.Restaurants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.OwnerId == request.UserId, cancellationToken);

                if (ownedRestaurant == null)
                {
                    return Result<MenuTemplateDto>.FailureResult("Restaurant not found for current user");
                }

                request.Template.RestaurantId = ownedRestaurant.Id;
            }
        }

        MenuTemplateMapper.UpdateEntity(entity, request.Template);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<MenuTemplateDto>.SuccessResult(MenuTemplateMapper.ToDto(entity), "Template updated");
    }
}

