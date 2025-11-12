using System.Text.Json;
using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.MenuTemplates.DTOs;
using Menufy.Application.Features.Restaurants.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.Restaurants.Commands.UpdateSettings;

public class UpdateRestaurantSettingsCommandHandler 
    : IRequestHandler<UpdateRestaurantSettingsCommand, Result<RestaurantSettingsDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateRestaurantSettingsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<RestaurantSettingsDto>> Handle(
        UpdateRestaurantSettingsCommand request, 
        CancellationToken cancellationToken)
    {
        var restaurant = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken);

        if (restaurant == null)
        {
            return Result<RestaurantSettingsDto>.FailureResult("Restaurant not found");
        }

        // Authorization check
        var isAdmin = string.Equals(request.UserRole, "Admin", StringComparison.OrdinalIgnoreCase);
        if (!isAdmin && restaurant.OwnerId != request.UserId)
        {
            return Result<RestaurantSettingsDto>.FailureResult("You are not authorized to update this restaurant's settings");
        }

        // Update active template
        if (request.Settings.ActiveTemplateId.HasValue)
        {
            // Validate template exists
            var templateExists = await _context.MenuTemplates
                .AnyAsync(t => t.Id == request.Settings.ActiveTemplateId.Value, cancellationToken);

            if (!templateExists)
            {
                return Result<RestaurantSettingsDto>.FailureResult("Template not found");
            }

            restaurant.ActiveTemplateId = request.Settings.ActiveTemplateId;
        }

        // Update custom theme
        if (request.Settings.CustomTheme != null)
        {
            restaurant.CustomTheme = JsonSerializer.Serialize(request.Settings.CustomTheme);
        }

        // Update display settings
        if (request.Settings.DisplaySettings != null)
        {
            restaurant.MenuDisplaySettings = JsonSerializer.Serialize(request.Settings.DisplaySettings);
        }

        // Update currency and language
        if (!string.IsNullOrWhiteSpace(request.Settings.Currency))
        {
            restaurant.Currency = request.Settings.Currency;
        }

        if (!string.IsNullOrWhiteSpace(request.Settings.DefaultLanguage))
        {
            restaurant.DefaultLanguage = request.Settings.DefaultLanguage;
        }

        restaurant.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        // Build response
        var response = new RestaurantSettingsDto
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

        return Result<RestaurantSettingsDto>.SuccessResult(response, "Settings updated successfully");
    }
}

