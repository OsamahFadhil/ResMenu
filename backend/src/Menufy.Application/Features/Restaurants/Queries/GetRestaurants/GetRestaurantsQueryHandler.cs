using AutoMapper;
using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Features.Restaurants.Common;
using Menufy.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.Restaurants.Queries.GetRestaurants;

public class GetRestaurantsQueryHandler : IRequestHandler<GetRestaurantsQuery, Result<PagedResult<RestaurantDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetRestaurantsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<RestaurantDto>>> Handle(GetRestaurantsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Restaurants
            .Include(r => r.Owner)
            .AsQueryable();

        // Filter by active status
        if (request.IsActive.HasValue)
        {
            query = query.Where(r => r.IsActive == request.IsActive.Value);
        }

        // Search
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(r =>
                r.Name.ToLower().Contains(searchTerm) ||
                r.Slug.ToLower().Contains(searchTerm) ||
                (r.ContactEmail != null && r.ContactEmail.ToLower().Contains(searchTerm)));
        }

        // Sort
        query = request.SortBy?.ToLower() switch
        {
            "name" => request.SortOrder.ToLower() == "desc"
                ? query.OrderByDescending(r => r.Name)
                : query.OrderBy(r => r.Name),
            "createdat" => request.SortOrder.ToLower() == "desc"
                ? query.OrderByDescending(r => r.CreatedAt)
                : query.OrderBy(r => r.CreatedAt),
            _ => query.OrderByDescending(r => r.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var restaurants = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var restaurantDtos = _mapper.Map<List<RestaurantDto>>(restaurants);

        var pagedResult = new PagedResult<RestaurantDto>(
            restaurantDtos,
            totalCount,
            request.Page,
            request.PageSize
        );

        return Result<PagedResult<RestaurantDto>>.SuccessResult(pagedResult);
    }
}
