using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.MenuCategories.Commands.ReorderCategories;

public class ReorderCategoriesCommandHandler
    : IRequestHandler<ReorderCategoriesCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public ReorderCategoriesCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(
        ReorderCategoriesCommand request,
        CancellationToken cancellationToken)
    {
        var categoryIds = request.NewOrder.Select(x => x.Id).ToList();

        var categories = await _context.MenuCategories
            .Where(c => c.RestaurantId == request.RestaurantId
                     && categoryIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        if (categories.Count != request.NewOrder.Count)
        {
            return Result<bool>.FailureResult("Invalid category IDs");
        }

        foreach (var orderDto in request.NewOrder)
        {
            var category = categories.FirstOrDefault(c => c.Id == orderDto.Id);
            if (category != null)
            {
                category.DisplayOrder = orderDto.DisplayOrder;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.SuccessResult(true, "Categories reordered successfully");
    }
}
