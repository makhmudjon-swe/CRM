using WholesaleCRM.Application.DTOs;

namespace WholesaleCRM.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync();
}
