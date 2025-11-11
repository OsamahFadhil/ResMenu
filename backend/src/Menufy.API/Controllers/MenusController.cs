using MediatR;
using Menufy.Application.Features.Menus.Commands.CreateCategory;
using Menufy.Application.Features.Menus.Commands.CreateMenuItem;
using Menufy.Application.Features.Menus.DTOs;
using Menufy.Application.Features.Menus.Queries.GetPublicMenu;
using Menufy.Application.Features.Menus.Queries.GetRestaurantCategories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Menufy.API.Controllers;

[ApiController]
[Route("api")]
public class MenusController : ControllerBase
{
    private readonly IMediator _mediator;

    public MenusController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("menu/{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicMenu(string slug)
    {
        var query = new GetPublicMenuQuery(slug);
        var result = await _mediator.Send(query);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpGet("restaurants/{restaurantId:guid}/categories")]
    [Authorize(Roles = "RestaurantOwner,Admin")]
    public async Task<IActionResult> GetCategories(Guid restaurantId)
    {
        var query = new GetRestaurantCategoriesQuery(restaurantId);
        var result = await _mediator.Send(query);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost("restaurants/{restaurantId:guid}/categories")]
    [Authorize(Roles = "RestaurantOwner,Admin")]
    public async Task<IActionResult> CreateCategory(Guid restaurantId, [FromBody] CreateCategoryDto dto)
    {
        var command = new CreateCategoryCommand(restaurantId, dto);
        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("categories/{categoryId:guid}/items")]
    [Authorize(Roles = "RestaurantOwner,Admin")]
    public async Task<IActionResult> CreateMenuItem(Guid categoryId, [FromBody] CreateMenuItemDto dto)
    {
        var command = new CreateMenuItemCommand(categoryId, dto);
        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
