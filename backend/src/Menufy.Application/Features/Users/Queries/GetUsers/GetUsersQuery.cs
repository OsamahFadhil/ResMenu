using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Auth.DTOs;
using Menufy.Domain.Common;

namespace Menufy.Application.Features.Users.Queries.GetUsers;

public class GetUsersQuery : IRequest<Result<PagedResult<UserDto>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
}
