using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Menus.DTOs;
using Menufy.Application.Features.Menus.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.Menus.Queries.GetRestaurantCategories;

public class GetRestaurantCategoriesQueryHandler : IRequestHandler<GetRestaurantCategoriesQuery, Result<List<MenuCategoryDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetRestaurantCategoriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<MenuCategoryDto>>> Handle(GetRestaurantCategoriesQuery request, CancellationToken cancellationToken)
    {
        var restaurantExists = await _context.Restaurants
            .AnyAsync(r => r.Id == request.RestaurantId, cancellationToken);

        if (!restaurantExists)
        {
            return Result<List<MenuCategoryDto>>.FailureResult("Restaurant not found");
        }

        var categories = await _context.MenuCategories
            .Where(c => c.RestaurantId == request.RestaurantId)
            .OrderBy(c => c.DisplayOrder)
            .Include(c => c.MenuItems)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var categoryDtos = MenuCategoryTreeBuilder.BuildTree(categories, request.Language);

        return Result<List<MenuCategoryDto>>.SuccessResult(categoryDtos, "Categories retrieved successfully");
    }
}

