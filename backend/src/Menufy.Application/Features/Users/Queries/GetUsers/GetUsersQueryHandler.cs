using AutoMapper;
using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Features.Auth.DTOs;
using Menufy.Domain.Common;
using Menufy.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<PagedResult<UserDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users.AsQueryable();

        // Filter by role
        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            if (Enum.TryParse<UserRole>(request.Role, true, out var role))
            {
                query = query.Where(u => u.Role == role);
            }
        }

        // Filter by active status
        if (request.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == request.IsActive.Value);
        }

        // Search
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(u =>
                u.Name.ToLower().Contains(searchTerm) ||
                u.Email.ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var userDtos = _mapper.Map<List<UserDto>>(users);

        var pagedResult = new PagedResult<UserDto>(
            userDtos,
            totalCount,
            request.Page,
            request.PageSize
        );

        return Result<PagedResult<UserDto>>.SuccessResult(pagedResult);
    }
}
