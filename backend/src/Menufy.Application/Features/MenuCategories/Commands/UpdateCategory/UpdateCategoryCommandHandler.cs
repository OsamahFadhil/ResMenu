using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;

namespace Menufy.Application.Features.MenuCategories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.MenuCategories
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category == null)
            return Result.FailureResult("Category not found");

        // Validate parent category if provided
        if (request.ParentCategoryId.HasValue)
        {
            var parentExists = await _context.MenuCategories
                .AnyAsync(c => c.Id == request.ParentCategoryId.Value && c.RestaurantId == category.RestaurantId, cancellationToken);

            if (!parentExists)
                return Result.FailureResult("Parent category not found");

            // Prevent circular reference
            if (request.ParentCategoryId.Value == request.Id)
                return Result.FailureResult("Category cannot be its own parent");
        }

        category.Name = request.Name;
        category.Translations = SerializeCategoryTranslations(request.Translations);
        category.DisplayOrder = request.DisplayOrder;
        category.ParentCategoryId = request.ParentCategoryId;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.SuccessResult();
    }

    private static string? SerializeCategoryTranslations(Dictionary<string, string>? translations)
    {
        if (translations == null || translations.Count == 0)
        {
            return null;
        }

        return JsonSerializer.Serialize(translations);
    }
}
