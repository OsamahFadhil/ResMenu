using System.Collections.Generic;

using MediatR;
using Menufy.Application.Common.Models;

namespace Menufy.Application.Features.MenuCategories.Commands.UpdateCategory;

public class UpdateCategoryCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string>? Translations { get; set; }
    public int DisplayOrder { get; set; }
    public Guid? ParentCategoryId { get; set; }
}
