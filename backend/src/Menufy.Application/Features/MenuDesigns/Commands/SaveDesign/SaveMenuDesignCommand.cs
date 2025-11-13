using MediatR;
using Menufy.Application.Features.MenuDesigns.DTOs;

namespace Menufy.Application.Features.MenuDesigns.Commands.SaveDesign;

public class SaveMenuDesignCommand : IRequest<SaveMenuDesignResultDto>
{
    public SaveMenuDesignDto Data { get; set; } = null!;
}

