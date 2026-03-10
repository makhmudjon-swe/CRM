using InventoryManagementService.Application.Interfaces;
using InventoryManagementService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagementService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<ILikeService, LikeService>();
        services.AddScoped<IInventoryAuthorizationService, InventoryAuthorizationService>();
        services.AddScoped<ICustomIdService, CustomIdService>();
        services.AddScoped<IInventoryAccessService, InventoryAccessService>();

        return services;
    }
}
