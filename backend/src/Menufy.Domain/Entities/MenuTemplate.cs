using Menufy.Domain.Common;

namespace Menufy.Domain.Entities;

public class MenuTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? RestaurantId { get; set; }
    public string Structure { get; set; } = string.Empty;
    public string? Theme { get; set; }
    public string? Tags { get; set; }
    public bool IsPublished { get; set; }

    public Restaurant? Restaurant { get; set; }
}

