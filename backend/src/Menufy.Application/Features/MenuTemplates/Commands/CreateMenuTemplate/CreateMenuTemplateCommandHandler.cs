using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.MenuTemplates.DTOs;
using Menufy.Application.Features.MenuTemplates.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.MenuTemplates.Commands.CreateMenuTemplate;

public class CreateMenuTemplateCommandHandler : IRequestHandler<CreateMenuTemplateCommand, Result<MenuTemplateDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateMenuTemplateCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<MenuTemplateDto>> Handle(CreateMenuTemplateCommand request, CancellationToken cancellationToken)
    {
        var isAdmin = string.Equals(request.UserRole, "Admin", StringComparison.OrdinalIgnoreCase);

        Guid? restaurantId = request.Template.RestaurantId;

        if (restaurantId.HasValue)
        {
            var restaurant = await _context.Restaurants
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == restaurantId.Value, cancellationToken);

            if (restaurant == null)
            {
                return Result<MenuTemplateDto>.FailureResult("Restaurant not found");
            }

            if (!isAdmin && restaurant.OwnerId != request.UserId)
            {
                return Result<MenuTemplateDto>.FailureResult("You are not allowed to create templates for this restaurant");
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

                restaurantId = ownedRestaurant.Id;
            }
        }

        request.Template.RestaurantId = restaurantId;

        var entity = MenuTemplateMapper.CreateEntity(request.Template);
        entity.Id = Guid.NewGuid();

        _context.MenuTemplates.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = MenuTemplateMapper.ToDto(entity);
        return Result<MenuTemplateDto>.SuccessResult(dto, "Template created");
    }
}

