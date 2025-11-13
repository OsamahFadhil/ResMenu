using MediatR;
using Menufy.Application.Features.Restaurants.Commands.UpdateRestaurant;
using Menufy.Application.Features.Restaurants.Queries.GetRestaurants;
using Menufy.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Menufy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RestaurantsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;

    public RestaurantsController(IMediator mediator, IApplicationDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRestaurants([FromQuery] GetRestaurantsQuery query)
    {
        var result = await _mediator.Send(query);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "RestaurantOwner,Admin")]
    public async Task<IActionResult> GetRestaurant(Guid id)
    {
        var restaurant = await _context.Restaurants
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new
            {
                r.Id,
                r.Name,
                r.Slug,
                r.LogoUrl,
                r.ContactPhone,
                r.ContactEmail,
                r.Address,
                r.Translations,
                r.MenuDisplaySettings,
                r.Currency,
                r.DefaultLanguage,
                r.ActiveTemplateId,
                r.CustomTheme,
                r.OwnerId,
                r.LastMenuUpdate,
                r.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (restaurant == null)
        {
            return NotFound(new { message = "Restaurant not found" });
        }

        return Ok(restaurant);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "RestaurantOwner,Admin")]
    public async Task<IActionResult> UpdateRestaurant(Guid id, [FromBody] UpdateRestaurantCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
