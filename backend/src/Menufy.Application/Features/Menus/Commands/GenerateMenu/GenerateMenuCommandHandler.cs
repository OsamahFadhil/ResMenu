using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Menus.DTOs;
using Menufy.Application.Features.MenuTemplates.DTOs;
using Menufy.Application.Features.MenuTemplates.Helpers;
using Menufy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Menufy.Application.Features.Menus.Commands.GenerateMenu;

public class GenerateMenuCommandHandler : IRequestHandler<GenerateMenuCommand, Result<MenuGenerationResultDto>>
{
    private readonly IApplicationDbContext _context;

    public GenerateMenuCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<MenuGenerationResultDto>> Handle(GenerateMenuCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken);

        if (restaurant == null)
        {
            return Result<MenuGenerationResultDto>.FailureResult("Restaurant not found");
        }

        var existingCategories = await _context.MenuCategories
            .Where(c => c.RestaurantId == request.RestaurantId)
            .Include(c => c.MenuItems)
            .ToListAsync(cancellationToken);

        if (existingCategories.Any() && !request.OverwriteExisting)
        {
            return Result<MenuGenerationResultDto>.FailureResult("Restaurant already has menu data. Set overwriteExisting to true to regenerate.");
        }

        if (existingCategories.Any())
        {
            _context.MenuCategories.RemoveRange(existingCategories);
            await _context.SaveChangesAsync(cancellationToken);
        }

        MenuTemplateStructureDto structure;
        string templateKey;

        if (request.TemplateId.HasValue)
        {
            var templateEntity = await _context.MenuTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == request.TemplateId.Value, cancellationToken);

            if (templateEntity == null)
            {
                return Result<MenuGenerationResultDto>.FailureResult("Template not found");
            }

            if (templateEntity.RestaurantId.HasValue && templateEntity.RestaurantId.Value != request.RestaurantId)
            {
                return Result<MenuGenerationResultDto>.FailureResult("Template does not belong to this restaurant");
            }

            var dto = MenuTemplateMapper.ToDto(templateEntity);
            structure = dto.Structure;
            templateKey = dto.Name;
        }
        else
        {
            var builtInTemplate = MenuGenerationTemplates.ResolveTemplate(request.TemplateKey);
            if (builtInTemplate == null)
            {
                return Result<MenuGenerationResultDto>.FailureResult("Template not found");
            }

            structure = MenuGenerationTemplates.ToStructure(builtInTemplate);
            templateKey = request.TemplateKey ?? "default";
        }

        if (request.TargetCategories.HasValue)
        {
            structure.Categories = structure.Categories
                .OrderBy(c => c.DisplayOrder)
                .Take(request.TargetCategories.Value)
                .ToList();
        }

        var now = DateTime.UtcNow;
        var categoryDisplayOrder = 1;
        var itemsCreated = 0;

        foreach (var categoryTemplate in structure.Categories.OrderBy(c => c.DisplayOrder))
        {
            var categoryId = Guid.NewGuid();
            var category = new MenuCategory
            {
                Id = categoryId,
                RestaurantId = request.RestaurantId,
                Name = categoryTemplate.Name,
                Translations = SerializeCategoryTranslations(categoryTemplate.Translations),
                DisplayOrder = categoryDisplayOrder++,
                CreatedAt = now,
                UpdatedAt = now
            };

            var items = categoryTemplate.Items
                .OrderBy(i => i.DisplayOrder)
                .ToList();

            if (request.TargetItemsPerCategory.HasValue)
            {
                items = items.Take(request.TargetItemsPerCategory.Value).ToList();
            }

            var itemDisplayOrder = 1;
            foreach (var itemTemplate in items)
            {
                var item = new MenuItem
                {
                    Id = Guid.NewGuid(),
                    CategoryId = categoryId,
                    Name = itemTemplate.Name,
                    Description = itemTemplate.Description,
                    Translations = SerializeMenuItemTranslations(itemTemplate.NameTranslations, itemTemplate.DescriptionTranslations),
                    Price = itemTemplate.Price,
                    ImageUrl = itemTemplate.ImageUrl,
                    IsAvailable = itemTemplate.IsAvailable,
                    DisplayOrder = itemDisplayOrder++,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                category.MenuItems.Add(item);
                itemsCreated++;
            }

            _context.MenuCategories.Add(category);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var resultDto = new MenuGenerationResultDto
        {
            RestaurantId = request.RestaurantId,
            CategoriesCreated = structure.Categories.Count,
            ItemsCreated = itemsCreated,
            OverwroteExisting = existingCategories.Any(),
            TemplateId = request.TemplateId,
            TemplateKey = templateKey,
            GeneratedAt = now
        };

        return Result<MenuGenerationResultDto>.SuccessResult(resultDto, "Menu generated successfully");
    }

    private static string? SerializeCategoryTranslations(Dictionary<string, string>? translations)
    {
        if (translations == null || translations.Count == 0)
        {
            return null;
        }

        return JsonSerializer.Serialize(translations);
    }

    private static string? SerializeMenuItemTranslations(
        Dictionary<string, string>? nameTranslations,
        Dictionary<string, string>? descriptionTranslations)
    {
        var keys = new HashSet<string>();

        if (nameTranslations != null)
        {
            foreach (var key in nameTranslations.Keys)
            {
                keys.Add(key);
            }
        }

        if (descriptionTranslations != null)
        {
            foreach (var key in descriptionTranslations.Keys)
            {
                keys.Add(key);
            }
        }

        if (keys.Count == 0)
        {
            return null;
        }

        var payload = new Dictionary<string, Dictionary<string, string?>>();

        foreach (var key in keys)
        {
            payload[key] = new Dictionary<string, string?>
            {
                ["name"] = nameTranslations != null && nameTranslations.TryGetValue(key, out var n) ? n : null,
                ["description"] = descriptionTranslations != null && descriptionTranslations.TryGetValue(key, out var d) ? d : null
            };
        }

        return JsonSerializer.Serialize(payload);
    }
}

