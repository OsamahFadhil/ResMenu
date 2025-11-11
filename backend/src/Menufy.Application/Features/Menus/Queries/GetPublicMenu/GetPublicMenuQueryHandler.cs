using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Menus.DTOs;
using Menufy.Application.Features.Menus.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.Menus.Queries.GetPublicMenu;

public class GetPublicMenuQueryHandler : IRequestHandler<GetPublicMenuQuery, Result<PublicMenuDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPublicMenuQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PublicMenuDto>> Handle(GetPublicMenuQuery request, CancellationToken cancellationToken)
    {
        var restaurant = await _context.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Slug == request.Slug, cancellationToken);

        if (restaurant == null)
        {
            return Result<PublicMenuDto>.FailureResult("Restaurant not found");
        }

        var categories = await _context.MenuCategories
            .Where(c => c.RestaurantId == restaurant.Id)
            .OrderBy(c => c.DisplayOrder)
            .Include(c => c.MenuItems)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var menuDto = new PublicMenuDto
        {
            RestaurantId = restaurant.Id,
            RestaurantName = restaurant.Name,
            LogoUrl = restaurant.LogoUrl,
            ContactPhone = restaurant.ContactPhone,
            ContactEmail = restaurant.ContactEmail,
            Address = restaurant.Address,
            Categories = MenuCategoryTreeBuilder.BuildTree(categories, item => item.IsAvailable)
        };

        return Result<PublicMenuDto>.SuccessResult(menuDto);
    }
}
