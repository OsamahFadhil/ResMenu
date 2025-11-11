using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.MenuItems.Commands.DeleteMenuItem;

public class DeleteMenuItemCommandHandler : IRequestHandler<DeleteMenuItemCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteMenuItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteMenuItemCommand request, CancellationToken cancellationToken)
    {
        var menuItem = await _context.MenuItems
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (menuItem == null)
            return Result.FailureResult("Menu item not found");

        _context.MenuItems.Remove(menuItem);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.SuccessResult();
    }
}
