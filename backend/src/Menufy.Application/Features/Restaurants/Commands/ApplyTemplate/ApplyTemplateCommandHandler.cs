using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Menus.Commands.GenerateMenu;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.Restaurants.Commands.ApplyTemplate;

public class ApplyTemplateCommandHandler : IRequestHandler<ApplyTemplateCommand, Result<ApplyTemplateResultDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public ApplyTemplateCommandHandler(IApplicationDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<Result<ApplyTemplateResultDto>> Handle(
        ApplyTemplateCommand request, 
        CancellationToken cancellationToken)
    {
        // Validate restaurant
        var restaurant = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken);

        if (restaurant == null)
        {
            return Result<ApplyTemplateResultDto>.FailureResult("Restaurant not found");
        }

        // Authorization check
        var isAdmin = string.Equals(request.UserRole, "Admin", StringComparison.OrdinalIgnoreCase);
        if (!isAdmin && restaurant.OwnerId != request.UserId)
        {
            return Result<ApplyTemplateResultDto>.FailureResult("You are not authorized to apply templates to this restaurant");
        }

        // Validate template
        var template = await _context.MenuTemplates
            .FirstOrDefaultAsync(t => t.Id == request.TemplateId, cancellationToken);

        if (template == null)
        {
            return Result<ApplyTemplateResultDto>.FailureResult("Template not found");
        }

        // Check if template belongs to another restaurant (not global and not owned by this restaurant)
        if (template.RestaurantId.HasValue && template.RestaurantId.Value != request.RestaurantId)
        {
            return Result<ApplyTemplateResultDto>.FailureResult("You cannot use templates from other restaurants");
        }

        // Generate menu from template
        var generateCommand = new GenerateMenuCommand(
            RestaurantId: request.RestaurantId,
            TemplateId: request.TemplateId,
            OverwriteExisting: request.OverwriteExisting,
            TemplateKey: null,
            TargetCategories: null,
            TargetItemsPerCategory: null,
            LanguageCodes: null
        );

        var generateResult = await _mediator.Send(generateCommand, cancellationToken);

        if (!generateResult.Success)
        {
            return Result<ApplyTemplateResultDto>.FailureResult(generateResult.Message);
        }

        // Update restaurant active template
        restaurant.ActiveTemplateId = request.TemplateId;
        restaurant.LastMenuUpdate = DateTime.UtcNow;

        // Copy template theme to restaurant's custom theme
        // This ensures the template's styling is immediately applied
        if (!string.IsNullOrWhiteSpace(template.Theme))
        {
            restaurant.CustomTheme = template.Theme;
        }

        // Update template usage stats
        template.UsageCount++;
        template.LastUsedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var result = new ApplyTemplateResultDto
        {
            RestaurantId = request.RestaurantId,
            TemplateId = request.TemplateId,
            CategoriesCreated = generateResult.Data!.CategoriesCreated,
            ItemsCreated = generateResult.Data.ItemsCreated,
            AppliedAt = DateTime.UtcNow
        };

        return Result<ApplyTemplateResultDto>.SuccessResult(result, "Template applied successfully");
    }
}

