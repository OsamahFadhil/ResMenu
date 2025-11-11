using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.MenuTemplates.DTOs;
using Menufy.Application.Features.MenuTemplates.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.MenuTemplates.Queries.GetMenuTemplates;

public class GetMenuTemplatesQueryHandler : IRequestHandler<GetMenuTemplatesQuery, Result<List<MenuTemplateDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetMenuTemplatesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<MenuTemplateDto>>> Handle(GetMenuTemplatesQuery request, CancellationToken cancellationToken)
    {
        var isAdmin = string.Equals(request.UserRole, "Admin", StringComparison.OrdinalIgnoreCase);

        var query = _context.MenuTemplates
            .AsNoTracking()
            .OrderBy(t => t.RestaurantId)
            .ThenBy(t => t.Name)
            .AsQueryable();

        if (!isAdmin)
        {
            var ownerRestaurantIds = await _context.Restaurants
                .AsNoTracking()
                .Where(r => r.OwnerId == request.UserId)
                .Select(r => r.Id)
                .ToListAsync(cancellationToken);

            query = query.Where(t => !t.RestaurantId.HasValue || ownerRestaurantIds.Contains(t.RestaurantId.Value));
        }

        var entities = await query.ToListAsync(cancellationToken);
        var dtos = entities.Select(MenuTemplateMapper.ToDto).ToList();

        return Result<List<MenuTemplateDto>>.SuccessResult(dtos);
    }
}

