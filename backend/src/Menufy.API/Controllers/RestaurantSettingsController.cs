using System.Security.Claims;
using MediatR;
using Menufy.Application.Features.Restaurants.Commands.ApplyTemplate;
using Menufy.Application.Features.Restaurants.Commands.UpdateSettings;
using Menufy.Application.Features.Restaurants.DTOs;
using Menufy.Application.Features.Restaurants.Queries.GetSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Menufy.API.Controllers;

[ApiController]
[Route("api/restaurants/{restaurantId:guid}")]
[Authorize(Roles = "RestaurantOwner,Admin")]
public class RestaurantSettingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RestaurantSettingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get restaurant menu settings
    /// </summary>
    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings(Guid restaurantId)
    {
        var (userId, role) = GetUserContext();
        var result = await _mediator.Send(new GetRestaurantSettingsQuery(restaurantId, userId, role));

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Update restaurant menu settings
    /// </summary>
    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSettings(
        Guid restaurantId, 
        [FromBody] UpdateRestaurantSettingsDto dto)
    {
        var (userId, role) = GetUserContext();
        var result = await _mediator.Send(
            new UpdateRestaurantSettingsCommand(restaurantId, userId, role, dto));

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Apply a template to restaurant menu
    /// </summary>
    [HttpPost("apply-template")]
    public async Task<IActionResult> ApplyTemplate(
        Guid restaurantId,
        [FromBody] ApplyTemplateRequest request)
    {
        var (userId, role) = GetUserContext();
        var command = new ApplyTemplateCommand(
            restaurantId,
            userId,
            role,
            request.TemplateId,
            request.OverwriteExisting);

        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    private (Guid UserId, string Role) GetUserContext()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User identifier not found");

        var role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        return (Guid.Parse(userIdClaim), role);
    }
}

public record ApplyTemplateRequest(Guid TemplateId, bool OverwriteExisting = false);

