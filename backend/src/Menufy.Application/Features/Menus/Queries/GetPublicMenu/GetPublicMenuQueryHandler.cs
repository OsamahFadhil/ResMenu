using System.Text.Json;
using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Menus.DTOs;
using Menufy.Application.Features.Menus.Helpers;
using Menufy.Application.Features.MenuTemplates.DTOs;
using Menufy.Application.Features.Restaurants.DTOs;
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
            .Include(r => r.ActiveTemplate)
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

        var categoriesDto = MenuCategoryTreeBuilder.BuildTree(
            categories,
            request.Language,
            item => item.IsAvailable);

        var localizedRestaurantName = string.IsNullOrWhiteSpace(request.Language)
            ? restaurant.GetTranslatedName()
            : restaurant.GetTranslatedName(request.Language);

        // Determine theme (priority: CustomTheme > ActiveTemplate.Theme > Default)
        MenuTemplateThemeDto? theme = null;
        if (!string.IsNullOrWhiteSpace(restaurant.CustomTheme))
        {
            try
            {
                theme = JsonSerializer.Deserialize<MenuTemplateThemeDto>(restaurant.CustomTheme);
            }
            catch (JsonException)
            {
                // Log error and continue with default
            }
        }
        else if (restaurant.ActiveTemplate?.Theme != null)
        {
            try
            {
                theme = JsonSerializer.Deserialize<MenuTemplateThemeDto>(restaurant.ActiveTemplate.Theme);
            }
            catch (JsonException)
            {
                // Log error and continue with default
            }
        }

        // Load display settings
        MenuDisplaySettingsDto? displaySettings = null;
        if (!string.IsNullOrWhiteSpace(restaurant.MenuDisplaySettings))
        {
            try
            {
                displaySettings = JsonSerializer.Deserialize<MenuDisplaySettingsDto>(restaurant.MenuDisplaySettings);
            }
            catch (JsonException)
            {
                displaySettings = new MenuDisplaySettingsDto();
            }
        }
        else
        {
            displaySettings = new MenuDisplaySettingsDto();
        }

        // Track menu view
        var restaurantToUpdate = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == restaurant.Id, cancellationToken);
        if (restaurantToUpdate != null)
        {
            restaurantToUpdate.TotalMenuViews++;
            await _context.SaveChangesAsync(cancellationToken);
        }

        var menuDto = new PublicMenuDto
        {
            RestaurantId = restaurant.Id,
            RestaurantName = restaurant.Name,
            RestaurantLocalizedName = localizedRestaurantName,
            LogoUrl = restaurant.LogoUrl,
            ContactPhone = restaurant.ContactPhone,
            ContactEmail = restaurant.ContactEmail,
            Address = restaurant.Address,
            Language = string.IsNullOrWhiteSpace(request.Language) ? "en" : request.Language!,
            Theme = theme,
            DisplaySettings = displaySettings,
            Currency = restaurant.Currency ?? "USD",
            Categories = categoriesDto
        };

        return Result<PublicMenuDto>.SuccessResult(menuDto);
    }
}
