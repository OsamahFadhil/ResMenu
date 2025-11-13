using MediatR;
using Menufy.Application.Features.MenuDesigns.DTOs;

namespace Menufy.Application.Features.MenuDesigns.Queries.GetDesign;

public class GetMenuDesignQuery : IRequest<MenuDesignDto?>
{
    public Guid RestaurantId { get; set; }
    public Guid? DesignId { get; set; } // If null, get the active design
}

