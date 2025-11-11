using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.Restaurants.Commands.UpdateRestaurant;

public class UpdateRestaurantCommandHandler : IRequestHandler<UpdateRestaurantCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateRestaurantCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateRestaurantCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (restaurant == null)
            return Result.FailureResult("Restaurant not found");

        restaurant.Name = request.Name;
        restaurant.LogoUrl = request.LogoUrl;
        restaurant.ContactPhone = request.ContactPhone;
        restaurant.ContactEmail = request.ContactEmail;
        restaurant.Address = request.Address;
        restaurant.Translations = request.Translations;
        restaurant.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.SuccessResult();
    }
}
