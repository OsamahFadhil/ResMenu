using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Menus.DTOs;
using Menufy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

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
            Translations = SerializeMenuItemTranslations(request.Dto.Translations),
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
            LocalizedName = menuItem.Name,
            Description = menuItem.Description,
            LocalizedDescription = menuItem.Description,
            Price = menuItem.Price,
            ImageUrl = menuItem.ImageUrl,
            IsAvailable = menuItem.IsAvailable,
            DisplayOrder = menuItem.DisplayOrder,
            Translations = request.Dto.Translations
        };

        return Result<MenuItemDto>.SuccessResult(dto, "Menu item created successfully");
    }

    private static string? SerializeMenuItemTranslations(Dictionary<string, MenuItemTranslationDto>? translations)
    {
        if (translations == null || translations.Count == 0)
        {
            return null;
        }

        var payload = translations.ToDictionary(
            kvp => kvp.Key,
            kvp => new Dictionary<string, string?>
            {
                ["name"] = kvp.Value.Name,
                ["description"] = kvp.Value.Description
            });

        return JsonSerializer.Serialize(payload);
    }
}
