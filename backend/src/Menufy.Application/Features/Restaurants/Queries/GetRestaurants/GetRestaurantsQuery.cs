using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Domain.Common;
using Menufy.Application.Features.Restaurants.Common;

namespace Menufy.Application.Features.Restaurants.Queries.GetRestaurants;

public class GetRestaurantsQuery : IRequest<Result<PagedResult<RestaurantDto>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public string SortOrder { get; set; } = "asc";
    public bool? IsActive { get; set; }
}
