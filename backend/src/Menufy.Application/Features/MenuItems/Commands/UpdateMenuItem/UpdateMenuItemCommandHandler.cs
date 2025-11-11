using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Features.Menus.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Menufy.Application.Features.MenuItems.Commands.UpdateMenuItem;

public class UpdateMenuItemCommandHandler : IRequestHandler<UpdateMenuItemCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateMenuItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var menuItem = await _context.MenuItems
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (menuItem == null)
            return Result.FailureResult("Menu item not found");

        menuItem.Name = request.Name;
        menuItem.Description = request.Description;
        menuItem.Translations = SerializeMenuItemTranslations(request.Translations);
        menuItem.Price = request.Price;
        menuItem.ImageUrl = request.ImageUrl;
        menuItem.IsAvailable = request.IsAvailable;
        menuItem.DisplayOrder = request.DisplayOrder;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.SuccessResult();
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
