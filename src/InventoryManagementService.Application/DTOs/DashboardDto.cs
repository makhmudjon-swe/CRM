namespace WholesaleCRM.Application.DTOs;

public class DashboardDto
{
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int ActiveDeals { get; set; }
    public int DealsWonThisMonth { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalProducts { get; set; }
    public int NewCustomersThisMonth { get; set; }
    public List<DealDto> RecentDeals { get; set; } = new();
    public List<ActivityDto> RecentActivities { get; set; } = new();
    public Dictionary<string, int> DealsByStatus { get; set; } = new();
    public Dictionary<string, decimal> RevenueByMonth { get; set; } = new();
}
