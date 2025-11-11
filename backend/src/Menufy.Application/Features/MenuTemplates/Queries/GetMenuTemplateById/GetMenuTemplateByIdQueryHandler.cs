using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.MenuTemplates.DTOs;
using Menufy.Application.Features.MenuTemplates.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.MenuTemplates.Queries.GetMenuTemplateById;

public class GetMenuTemplateByIdQueryHandler : IRequestHandler<GetMenuTemplateByIdQuery, Result<MenuTemplateDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMenuTemplateByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<MenuTemplateDto>> Handle(GetMenuTemplateByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.MenuTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            return Result<MenuTemplateDto>.FailureResult("Template not found");
        }

        var isAdmin = string.Equals(request.UserRole, "Admin", StringComparison.OrdinalIgnoreCase);

        if (!isAdmin && entity.RestaurantId.HasValue)
        {
            var restaurant = await _context.Restaurants
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == entity.RestaurantId.Value, cancellationToken);

            if (restaurant == null || restaurant.OwnerId != request.UserId)
            {
                return Result<MenuTemplateDto>.FailureResult("You are not allowed to access this template");
            }
        }

        return Result<MenuTemplateDto>.SuccessResult(MenuTemplateMapper.ToDto(entity));
    }
}

