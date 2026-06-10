using Microsoft.EntityFrameworkCore;
using WholesaleCRM.Application.DTOs;
using WholesaleCRM.Application.Interfaces;
using WholesaleCRM.Domain.Enums;
using WholesaleCRM.Domain.Interfaces;

namespace WholesaleCRM.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _uow;
    private readonly IActivityService _activityService;
    private readonly IDealService _dealService;

    public DashboardService(IUnitOfWork uow, IActivityService activityService, IDealService dealService)
    {
        _uow = uow;
        _activityService = activityService;
        _dealService = dealService;
    }

    public async Task<DashboardDto> GetDashboardAsync()
    {
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var totalCustomers = await _uow.Customers.Query().CountAsync();
        var activeCustomers = await _uow.Customers.Query()
            .CountAsync(c => c.Status == CustomerStatus.Active);
        var newCustomersThisMonth = await _uow.Customers.Query()
            .CountAsync(c => c.CreatedAt >= monthStart);
        var totalProducts = await _uow.Products.Query().CountAsync(p => p.IsActive);

        var activeDeals = await _uow.Deals.Query()
            .CountAsync(d => d.Status != DealStatus.Won && d.Status != DealStatus.Lost);
        var dealsWonThisMonth = await _uow.Deals.Query()
            .CountAsync(d => d.Status == DealStatus.Won && d.UpdatedAt >= monthStart);

        var monthlyRevenue = await _uow.Deals.Query()
            .Where(d => d.Status == DealStatus.Won && d.UpdatedAt >= monthStart)
            .SumAsync(d => (decimal?)d.TotalAmount) ?? 0;
        var totalRevenue = await _uow.Deals.Query()
            .Where(d => d.Status == DealStatus.Won)
            .SumAsync(d => (decimal?)d.TotalAmount) ?? 0;

        var dealsByStatus = await _uow.Deals.Query()
            .GroupBy(d => d.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        var revenueByMonth = await _uow.Deals.Query()
            .Where(d => d.Status == DealStatus.Won && d.UpdatedAt >= now.AddMonths(-6))
            .GroupBy(d => new { d.UpdatedAt.Year, d.UpdatedAt.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Sum(d => d.TotalAmount) })
            .OrderBy(g => g.Year).ThenBy(g => g.Month)
            .ToListAsync();

        var recentDeals = (await _dealService.GetAllAsync()).Take(5).ToList();
        var recentActivities = (await _activityService.GetRecentAsync(8)).ToList();

        return new DashboardDto
        {
            TotalCustomers = totalCustomers,
            ActiveCustomers = activeCustomers,
            ActiveDeals = activeDeals,
            DealsWonThisMonth = dealsWonThisMonth,
            MonthlyRevenue = monthlyRevenue,
            TotalRevenue = totalRevenue,
            TotalProducts = totalProducts,
            NewCustomersThisMonth = newCustomersThisMonth,
            RecentDeals = recentDeals,
            RecentActivities = recentActivities,
            DealsByStatus = dealsByStatus.ToDictionary(
                x => EnumHelper.GetDealStatusName((int)x.Status),
                x => x.Count),
            RevenueByMonth = revenueByMonth.ToDictionary(
                x => $"{x.Year}-{x.Month:D2}",
                x => x.Total)
        };
    }
}
