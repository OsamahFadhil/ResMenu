using MediatR;
using Menufy.Application.Common.Models;

namespace Menufy.Application.Features.MenuCategories.Commands.DeleteCategory;

public class DeleteCategoryCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
