using System.Text.Json;
using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.MenuTemplates.DTOs;
using Menufy.Application.Features.Restaurants.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.Restaurants.Queries.GetSettings;

public class GetRestaurantSettingsQueryHandler 
    : IRequestHandler<GetRestaurantSettingsQuery, Result<RestaurantSettingsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRestaurantSettingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<RestaurantSettingsDto>> Handle(
        GetRestaurantSettingsQuery request, 
        CancellationToken cancellationToken)
    {
        var restaurant = await _context.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken);

        if (restaurant == null)
        {
            return Result<RestaurantSettingsDto>.FailureResult("Restaurant not found");
        }

        // Authorization check
        var isAdmin = string.Equals(request.UserRole, "Admin", StringComparison.OrdinalIgnoreCase);
        if (!isAdmin && restaurant.OwnerId != request.UserId)
        {
            return Result<RestaurantSettingsDto>.FailureResult("You are not authorized to view this restaurant's settings");
        }

        var settings = new RestaurantSettingsDto
        {
            RestaurantId = restaurant.Id,
            ActiveTemplateId = restaurant.ActiveTemplateId,
            CustomTheme = restaurant.CustomTheme != null
                ? JsonSerializer.Deserialize<MenuTemplateThemeDto>(restaurant.CustomTheme)
                : null,
            DisplaySettings = restaurant.MenuDisplaySettings != null
                ? JsonSerializer.Deserialize<MenuDisplaySettingsDto>(restaurant.MenuDisplaySettings)!
                : new MenuDisplaySettingsDto(),
            Currency = restaurant.Currency ?? "USD",
            DefaultLanguage = restaurant.DefaultLanguage ?? "en"
        };

        return Result<RestaurantSettingsDto>.SuccessResult(settings);
    }
}

