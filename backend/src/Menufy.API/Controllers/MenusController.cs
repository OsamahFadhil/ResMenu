using MediatR;
using Menufy.API.Contracts;
using Menufy.Application.Features.MenuCategories.Commands.DeleteCategory;
using Menufy.Application.Features.MenuCategories.Commands.UpdateCategory;
using Menufy.Application.Features.Menus.Commands.CreateCategory;
using Menufy.Application.Features.Menus.Commands.CreateMenuItem;
using Menufy.Application.Features.Menus.Commands.GenerateMenu;
using Menufy.Application.Features.MenuItems.Commands.DeleteMenuItem;
using Menufy.Application.Features.MenuItems.Commands.UpdateMenuItem;
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
    public async Task<IActionResult> GetPublicMenu(string slug, [FromQuery] string? lang = null)
    {
        var query = new GetPublicMenuQuery(slug, lang);
        var result = await _mediator.Send(query);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpGet("restaurants/{restaurantId:guid}/categories")]
    [Authorize(Roles = "RestaurantOwner,Admin")]
    public async Task<IActionResult> GetCategories(Guid restaurantId, [FromQuery] string? lang = null)
    {
        var query = new GetRestaurantCategoriesQuery(restaurantId, lang);
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

    [HttpPut("categories/{id:guid}")]
    [Authorize(Roles = "RestaurantOwner,Admin")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("categories/{id:guid}")]
    [Authorize(Roles = "RestaurantOwner,Admin")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var command = new DeleteCategoryCommand { Id = id };
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

    [HttpPost("restaurants/{restaurantId:guid}/menu/generate")]
    [Authorize(Roles = "RestaurantOwner,Admin")]
    public async Task<IActionResult> GenerateMenu(Guid restaurantId, [FromBody] GenerateMenuRequest? request)
    {
        request ??= new GenerateMenuRequest();
        var command = new GenerateMenuCommand(
            restaurantId,
            request.OverwriteExisting,
            request.TemplateKey,
            request.TemplateId,
            request.LanguageCodes,
            request.TargetCategories,
            request.TargetItemsPerCategory);

        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("items/{id:guid}")]
    [Authorize(Roles = "RestaurantOwner,Admin")]
    public async Task<IActionResult> UpdateMenuItem(Guid id, [FromBody] UpdateMenuItemCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("items/{id:guid}")]
    [Authorize(Roles = "RestaurantOwner,Admin")]
    public async Task<IActionResult> DeleteMenuItem(Guid id)
    {
        var command = new DeleteMenuItemCommand { Id = id };
        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
