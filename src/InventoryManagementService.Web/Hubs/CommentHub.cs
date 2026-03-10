using Microsoft.AspNetCore.SignalR;

namespace InventoryManagementService.Web.Hubs;

public class CommentHub : Hub
{
    public async Task JoinItemGroup(int itemId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"item-{itemId}");
    }

    public async Task LeaveItemGroup(int itemId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"item-{itemId}");
    }
}
