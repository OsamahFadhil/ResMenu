using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Menus.DTOs;
using Menufy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.Menus.Commands.CreateMenuItem;

public class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand, Result<MenuItemDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateMenuItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<MenuItemDto>> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var categoryExists = await _context.MenuCategories
            .AnyAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (!categoryExists)
        {
            return Result<MenuItemDto>.FailureResult("Category not found");
        }

        var menuItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = request.Dto.Name,
            Description = request.Dto.Description,
            Price = request.Dto.Price,
            ImageUrl = request.Dto.ImageUrl,
            IsAvailable = request.Dto.IsAvailable,
            DisplayOrder = request.Dto.DisplayOrder,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new MenuItemDto
        {
            Id = menuItem.Id,
            Name = menuItem.Name,
            Description = menuItem.Description,
            Price = menuItem.Price,
            ImageUrl = menuItem.ImageUrl,
            IsAvailable = menuItem.IsAvailable,
            DisplayOrder = menuItem.DisplayOrder
        };

        return Result<MenuItemDto>.SuccessResult(dto, "Menu item created successfully");
    }
}
