using MediatR;
using Menufy.Application.Common.Models;

namespace Menufy.Application.Features.MenuItems.Commands.ReorderItems;

public record ReorderMenuItemsCommand(
    Guid CategoryId,
    List<ItemOrderDto> NewOrder) : IRequest<Result<bool>>;

public record ItemOrderDto(Guid Id, int DisplayOrder);
