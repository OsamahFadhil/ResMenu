using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Features.MenuDesigns.DTOs;
using Menufy.Domain.Entities;

namespace Menufy.Application.Features.MenuDesigns.Commands.SaveDesign;

public class SaveMenuDesignCommandHandler : IRequestHandler<SaveMenuDesignCommand, SaveMenuDesignResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<SaveMenuDesignCommandHandler> _logger;

    public SaveMenuDesignCommandHandler(
        IApplicationDbContext context,
        ILogger<SaveMenuDesignCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SaveMenuDesignResultDto> Handle(SaveMenuDesignCommand request, CancellationToken cancellationToken)
    {
        var data = request.Data;

        _logger.LogInformation("Saving menu design for restaurant {RestaurantId}", data.RestaurantId);

        // Verify restaurant exists
        var restaurant = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == data.RestaurantId, cancellationToken);

        if (restaurant == null)
        {
            throw new InvalidOperationException($"Restaurant {data.RestaurantId} not found");
        }

        // If setting as active, deactivate all other designs for this restaurant
        if (data.SetAsActive)
        {
            var existingDesigns = await _context.MenuDesigns
                .Where(md => md.RestaurantId == data.RestaurantId && md.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var design in existingDesigns)
            {
                design.IsActive = false;
            }
        }

        // Get the latest version number for this restaurant
        var latestVersion = await _context.MenuDesigns
            .Where(md => md.RestaurantId == data.RestaurantId)
            .OrderByDescending(md => md.Version)
            .Select(md => md.Version)
            .FirstOrDefaultAsync(cancellationToken);

        // Create new design
        var menuDesign = new MenuDesign
        {
            Id = Guid.NewGuid(),
            RestaurantId = data.RestaurantId,
            LayoutConfiguration = JsonSerializer.Serialize(data.LayoutConfiguration),
            GlobalTheme = JsonSerializer.Serialize(data.GlobalTheme),
            Version = latestVersion + 1,
            IsActive = data.SetAsActive,
            Name = data.Name
        };

        _context.MenuDesigns.Add(menuDesign);

        // Update restaurant's last menu update timestamp
        restaurant.LastMenuUpdate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Menu design saved successfully: {DesignId}, Version: {Version}", 
            menuDesign.Id, menuDesign.Version);

        return new SaveMenuDesignResultDto
        {
            Id = menuDesign.Id,
            Version = menuDesign.Version,
            IsActive = menuDesign.IsActive,
            Message = data.SetAsActive 
                ? "Design saved and activated successfully" 
                : "Design saved successfully"
        };
    }
}

