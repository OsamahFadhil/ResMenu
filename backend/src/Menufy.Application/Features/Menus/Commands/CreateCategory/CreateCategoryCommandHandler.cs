using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Menus.DTOs;
using Menufy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.Menus.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<MenuCategoryDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<MenuCategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var restaurantExists = await _context.Restaurants
            .AnyAsync(r => r.Id == request.RestaurantId, cancellationToken);

        if (!restaurantExists)
        {
            return Result<MenuCategoryDto>.FailureResult("Restaurant not found");
        }

        Guid? parentCategoryId = null;

        if (request.Dto.ParentCategoryId.HasValue)
        {
            var parent = await _context.MenuCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.Dto.ParentCategoryId.Value, cancellationToken);

            if (parent == null || parent.RestaurantId != request.RestaurantId)
            {
                return Result<MenuCategoryDto>.FailureResult("Parent category not found for this restaurant");
            }

            parentCategoryId = parent.Id;
        }

        var category = new MenuCategory
        {
            Id = Guid.NewGuid(),
            Name = request.Dto.Name,
            DisplayOrder = request.Dto.DisplayOrder,
            RestaurantId = request.RestaurantId,
            ParentCategoryId = parentCategoryId,
            CreatedAt = DateTime.UtcNow
        };

        _context.MenuCategories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new MenuCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            DisplayOrder = category.DisplayOrder,
            ParentCategoryId = category.ParentCategoryId,
            Children = new List<MenuCategoryDto>(),
            Items = new List<MenuItemDto>()
        };

        return Result<MenuCategoryDto>.SuccessResult(dto, "Category created successfully");
    }
}
