using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.Admin.Queries.GetDashboardAnalytics;

public class GetDashboardAnalyticsQueryHandler : IRequestHandler<GetDashboardAnalyticsQuery, Result<DashboardAnalyticsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetDashboardAnalyticsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DashboardAnalyticsDto>> Handle(GetDashboardAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var totalRestaurants = await _context.Restaurants.CountAsync(cancellationToken);
        var activeRestaurants = await _context.Restaurants.CountAsync(r => r.IsActive, cancellationToken);
        var totalUsers = await _context.Users.CountAsync(cancellationToken);
        var totalMenuItems = await _context.MenuItems.CountAsync(cancellationToken);
        var totalCategories = await _context.MenuCategories.CountAsync(cancellationToken);
        var totalQRScans = await _context.QRCodes.CountAsync(cancellationToken);

        // Get monthly statistics for last 6 months
        var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);

        var monthlyRestaurants = await _context.Restaurants
            .Where(r => r.CreatedAt >= sixMonthsAgo)
            .GroupBy(r => new { r.CreatedAt.Year, r.CreatedAt.Month })
            .Select(g => new MonthlyStatDto
            {
                Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        var monthlyUsers = await _context.Users
            .Where(u => u.CreatedAt >= sixMonthsAgo)
            .GroupBy(u => new { u.CreatedAt.Year, u.CreatedAt.Month })
            .Select(g => new MonthlyStatDto
            {
                Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        var analytics = new DashboardAnalyticsDto
        {
            TotalRestaurants = totalRestaurants,
            ActiveRestaurants = activeRestaurants,
            TotalUsers = totalUsers,
            TotalMenuItems = totalMenuItems,
            TotalCategories = totalCategories,
            TotalQRScans = totalQRScans,
            MonthlyRestaurants = monthlyRestaurants,
            MonthlyUsers = monthlyUsers
        };

        return Result<DashboardAnalyticsDto>.SuccessResult(analytics);
    }
}
