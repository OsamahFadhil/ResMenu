using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Menus.DTOs;

namespace Menufy.Application.Features.Menus.Queries.GetPublicMenu;

public record GetPublicMenuQuery(string Slug, string? Language = null) : IRequest<Result<PublicMenuDto>>;
