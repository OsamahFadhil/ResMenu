using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.MenuCategories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.MenuCategories
            .Include(c => c.Children)
            .Include(c => c.MenuItems)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category == null)
            return Result.FailureResult("Category not found");

        // Check if category has children or items
        if (category.Children.Any())
            return Result.FailureResult("Cannot delete category with subcategories. Please delete subcategories first.");

        if (category.MenuItems.Any())
            return Result.FailureResult("Cannot delete category with menu items. Please move or delete items first.");

        _context.MenuCategories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.SuccessResult();
    }
}
