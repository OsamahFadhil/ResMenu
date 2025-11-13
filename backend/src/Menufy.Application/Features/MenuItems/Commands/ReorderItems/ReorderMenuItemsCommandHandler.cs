using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.MenuItems.Commands.ReorderItems;

public class ReorderMenuItemsCommandHandler
    : IRequestHandler<ReorderMenuItemsCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public ReorderMenuItemsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(
        ReorderMenuItemsCommand request,
        CancellationToken cancellationToken)
    {
        var itemIds = request.NewOrder.Select(x => x.Id).ToList();

        var items = await _context.MenuItems
            .Where(i => i.CategoryId == request.CategoryId
                     && itemIds.Contains(i.Id))
            .ToListAsync(cancellationToken);

        if (items.Count != request.NewOrder.Count)
        {
            return Result<bool>.FailureResult("Invalid item IDs");
        }

        foreach (var orderDto in request.NewOrder)
        {
            var item = items.FirstOrDefault(i => i.Id == orderDto.Id);
            if (item != null)
            {
                item.DisplayOrder = orderDto.DisplayOrder;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.SuccessResult(true, "Items reordered successfully");
    }
}
