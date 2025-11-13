using MediatR;
using Menufy.Application.Common.Models;

namespace Menufy.Application.Features.MenuCategories.Commands.ReorderCategories;

public record ReorderCategoriesCommand(
    Guid RestaurantId,
    List<CategoryOrderDto> NewOrder) : IRequest<Result<bool>>;

public record CategoryOrderDto(Guid Id, int DisplayOrder);
