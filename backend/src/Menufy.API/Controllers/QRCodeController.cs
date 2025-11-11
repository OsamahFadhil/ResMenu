using MediatR;
using Menufy.Application.Features.QRCodes.Commands.GenerateQRCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Menufy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "RestaurantOwner,Admin")]
public class QRCodeController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public QRCodeController(IMediator mediator, IConfiguration configuration)
    {
        _mediator = mediator;
        _configuration = configuration;
    }

    [HttpGet("{restaurantId:guid}")]
    public async Task<IActionResult> GenerateQRCode(Guid restaurantId)
    {
        var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "http://localhost:3000";
        var command = new GenerateQRCodeCommand(restaurantId, baseUrl);
        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
