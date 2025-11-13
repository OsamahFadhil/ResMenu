using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Features.MenuDesigns.DTOs;
using Menufy.Application.Features.MenuTemplates.DTOs;

namespace Menufy.Application.Features.MenuDesigns.Queries.GetDesign;

public class GetMenuDesignQueryHandler : IRequestHandler<GetMenuDesignQuery, MenuDesignDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetMenuDesignQueryHandler> _logger;

    public GetMenuDesignQueryHandler(
        IApplicationDbContext context,
        ILogger<GetMenuDesignQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MenuDesignDto?> Handle(GetMenuDesignQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting menu design for restaurant {RestaurantId}, DesignId: {DesignId}", 
            request.RestaurantId, request.DesignId);

        var query = _context.MenuDesigns
            .Where(md => md.RestaurantId == request.RestaurantId);

        if (request.DesignId.HasValue)
        {
            query = query.Where(md => md.Id == request.DesignId.Value);
        }
        else
        {
            // Get the active design
            query = query.Where(md => md.IsActive);
        }

        var design = await query
            .OrderByDescending(md => md.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (design == null)
        {
            _logger.LogWarning("No menu design found for restaurant {RestaurantId}", request.RestaurantId);
            return null;
        }

        return new MenuDesignDto
        {
            Id = design.Id,
            RestaurantId = design.RestaurantId,
            LayoutConfiguration = JsonSerializer.Deserialize<LayoutConfigurationDto>(design.LayoutConfiguration) 
                ?? new LayoutConfigurationDto(),
            GlobalTheme = JsonSerializer.Deserialize<MenuTemplateThemeDto>(design.GlobalTheme) 
                ?? new MenuTemplateThemeDto(),
            Version = design.Version,
            IsActive = design.IsActive,
            Name = design.Name,
            CreatedAt = design.CreatedAt,
            UpdatedAt = design.UpdatedAt ?? design.CreatedAt
        };
    }
}

