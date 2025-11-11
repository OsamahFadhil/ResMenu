using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Menus.DTOs;

namespace Menufy.Application.Features.Menus.Commands.GenerateMenu;

public record GenerateMenuCommand(
    Guid RestaurantId,
    bool OverwriteExisting = false,
    string? TemplateKey = "default",
    Guid? TemplateId = null,
    List<string>? LanguageCodes = null,
    int? TargetCategories = null,
    int? TargetItemsPerCategory = null) : IRequest<Result<MenuGenerationResultDto>>;

