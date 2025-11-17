using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Menufy.Application.Features.MenuDesigns.Commands.SaveDesign;
using Menufy.Application.Features.MenuDesigns.Queries.GetDesign;
using Menufy.Application.Features.MenuDesigns.DTOs;
using System.Security.Claims;

namespace Menufy.API.Controllers;

[ApiController]
[Route("api/menu-design")]
[Authorize]
public class MenuDesignController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<MenuDesignController> _logger;

    public MenuDesignController(IMediator mediator, ILogger<MenuDesignController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get the active menu design for a restaurant
    /// </summary>
    [HttpGet("restaurant/{restaurantId}")]
    public async Task<ActionResult<MenuDesignDto>> GetActiveDesign(Guid restaurantId)
    {
        _logger.LogInformation("Getting active menu design for restaurant {RestaurantId}", restaurantId);

        var result = await _mediator.Send(new GetMenuDesignQuery
        {
            RestaurantId = restaurantId
        });

        if (result == null)
        {
            return NotFound(new { message = "No active menu design found for this restaurant" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Get a specific menu design by ID
    /// </summary>
    [HttpGet("{designId}")]
    public async Task<ActionResult<MenuDesignDto>> GetDesignById(Guid designId, [FromQuery] Guid restaurantId)
    {
        _logger.LogInformation("Getting menu design {DesignId} for restaurant {RestaurantId}", 
            designId, restaurantId);

        var result = await _mediator.Send(new GetMenuDesignQuery
        {
            RestaurantId = restaurantId,
            DesignId = designId
        });

        if (result == null)
        {
            return NotFound(new { message = "Menu design not found" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Save a new menu design (creates a new version)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SaveMenuDesignResultDto>> SaveDesign([FromBody] SaveMenuDesignDto data)
    {
        _logger.LogInformation("Saving new menu design for restaurant {RestaurantId}", data.RestaurantId);
        _logger.LogInformation("Header settings - Color: {HeaderColor}, ImageUrl: {HeaderImageUrl}, DisplayMode: {HeaderDisplayMode}", 
            data.HeaderColor ?? "null", 
            data.HeaderImageUrl ?? "null", 
            data.HeaderDisplayMode?.ToString() ?? "null");

        try
        {
            var result = await _mediator.Send(new SaveMenuDesignCommand
            {
                Data = data
            });

            return CreatedAtAction(
                nameof(GetDesignById),
                new { designId = result.Id, restaurantId = data.RestaurantId },
                result
            );
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error saving menu design");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error saving menu design");
            return StatusCode(500, new { message = "An error occurred while saving the design" });
        }
    }

    /// <summary>
    /// Update an existing menu design (creates a new version)
    /// </summary>
    [HttpPut]
    public async Task<ActionResult<SaveMenuDesignResultDto>> UpdateDesign([FromBody] SaveMenuDesignDto data)
    {
        // In this system, "updating" just creates a new version
        return await SaveDesign(data);
    }
}

