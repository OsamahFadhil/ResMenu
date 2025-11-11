using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Menus.DTOs;

namespace Menufy.Application.Features.Menus.Queries.GetRestaurantCategories;

public record GetRestaurantCategoriesQuery(Guid RestaurantId, string? Language = null) : IRequest<Result<List<MenuCategoryDto>>>;

