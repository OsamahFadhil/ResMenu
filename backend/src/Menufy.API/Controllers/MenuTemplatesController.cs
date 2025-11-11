using System.Security.Claims;
using MediatR;
using Menufy.Application.Features.MenuTemplates.Commands.CreateMenuTemplate;
using Menufy.Application.Features.MenuTemplates.Commands.DeleteMenuTemplate;
using Menufy.Application.Features.MenuTemplates.Commands.UpdateMenuTemplate;
using Menufy.Application.Features.MenuTemplates.DTOs;
using Menufy.Application.Features.MenuTemplates.Queries.GetMenuTemplateById;
using Menufy.Application.Features.MenuTemplates.Queries.GetMenuTemplates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Menufy.API.Controllers;

[ApiController]
[Route("api/menu-templates")]
[Authorize(Roles = "RestaurantOwner,Admin")]
public class MenuTemplatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MenuTemplatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetTemplates()
    {
        var (userId, role) = GetUserContext();
        var result = await _mediator.Send(new GetMenuTemplatesQuery(userId, role));

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTemplate(Guid id)
    {
        var (userId, role) = GetUserContext();
        var result = await _mediator.Send(new GetMenuTemplateByIdQuery(id, userId, role));

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTemplate([FromBody] UpsertMenuTemplateDto dto)
    {
        var (userId, role) = GetUserContext();
        var result = await _mediator.Send(new CreateMenuTemplateCommand(userId, role, dto));

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTemplate(Guid id, [FromBody] UpsertMenuTemplateDto dto)
    {
        var (userId, role) = GetUserContext();
        var result = await _mediator.Send(new UpdateMenuTemplateCommand(id, userId, role, dto));

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTemplate(Guid id)
    {
        var (userId, role) = GetUserContext();
        var result = await _mediator.Send(new DeleteMenuTemplateCommand(id, userId, role));

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

