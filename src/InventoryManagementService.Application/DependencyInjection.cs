using Microsoft.Extensions.DependencyInjection;
using WholesaleCRM.Application.Interfaces;
using WholesaleCRM.Application.Services;

namespace WholesaleCRM.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IDealService, DealService>();
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<IDashboardService, DashboardService>();
        return services;
    }
}
