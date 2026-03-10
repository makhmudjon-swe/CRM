using InventoryManagementService.Domain.Entities;

namespace InventoryManagementService.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Inventory> Inventories { get; }
    IRepository<Item> Items { get; }
    IRepository<Category> Categories { get; }
    IRepository<Tag> Tags { get; }
    IRepository<InventoryTag> InventoryTags { get; }
    IRepository<Comment> Comments { get; }
    IRepository<Like> Likes { get; }
    IRepository<InventoryAccess> InventoryAccesses { get; }
    IRepository<CustomIdFormat> CustomIdFormats { get; }
    IRepository<CustomIdElement> CustomIdElements { get; }
    Task<int> SaveChangesAsync();
}
