using MediatR;
using Menufy.Application.Common.Models;

namespace Menufy.Application.Features.Restaurants.Commands.ApplyTemplate;

public record ApplyTemplateCommand(
    Guid RestaurantId,
    Guid UserId,
    string UserRole,
    Guid TemplateId,
    bool OverwriteExisting = false
) : IRequest<Result<ApplyTemplateResultDto>>;

public class ApplyTemplateResultDto
{
    public Guid RestaurantId { get; set; }
    public Guid TemplateId { get; set; }
    public int CategoriesCreated { get; set; }
    public int ItemsCreated { get; set; }
    public DateTime AppliedAt { get; set; }
}

