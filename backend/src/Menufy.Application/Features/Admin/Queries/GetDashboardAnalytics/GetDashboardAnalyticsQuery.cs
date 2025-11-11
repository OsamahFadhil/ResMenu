using MediatR;
using Menufy.Application.Common.Models;

namespace Menufy.Application.Features.Admin.Queries.GetDashboardAnalytics;

public class GetDashboardAnalyticsQuery : IRequest<Result<DashboardAnalyticsDto>>
{
}

public class DashboardAnalyticsDto
{
    public int TotalRestaurants { get; set; }
    public int ActiveRestaurants { get; set; }
    public int TotalUsers { get; set; }
    public int TotalMenuItems { get; set; }
    public int TotalCategories { get; set; }
    public int TotalQRScans { get; set; }
    public List<MonthlyStatDto> MonthlyRestaurants { get; set; } = new();
    public List<MonthlyStatDto> MonthlyUsers { get; set; } = new();
}

public class MonthlyStatDto
{
    public string Month { get; set; } = string.Empty;
    public int Count { get; set; }
}
