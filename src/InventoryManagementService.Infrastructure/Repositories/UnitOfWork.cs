using InventoryManagementService.Domain.Entities;
using InventoryManagementService.Domain.Interfaces;
using InventoryManagementService.Infrastructure.Data;

namespace InventoryManagementService.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private IRepository<Inventory>? _inventories;
    private IRepository<Item>? _items;
    private IRepository<Category>? _categories;
    private IRepository<Tag>? _tags;
    private IRepository<InventoryTag>? _inventoryTags;
    private IRepository<Comment>? _comments;
    private IRepository<Like>? _likes;
    private IRepository<InventoryAccess>? _inventoryAccesses;
    private IRepository<CustomIdFormat>? _customIdFormats;
    private IRepository<CustomIdElement>? _customIdElements;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IRepository<Inventory> Inventories => _inventories ??= new Repository<Inventory>(_context);
    public IRepository<Item> Items => _items ??= new Repository<Item>(_context);
    public IRepository<Category> Categories => _categories ??= new Repository<Category>(_context);
    public IRepository<Tag> Tags => _tags ??= new Repository<Tag>(_context);
    public IRepository<InventoryTag> InventoryTags => _inventoryTags ??= new Repository<InventoryTag>(_context);
    public IRepository<Comment> Comments => _comments ??= new Repository<Comment>(_context);
    public IRepository<Like> Likes => _likes ??= new Repository<Like>(_context);
    public IRepository<InventoryAccess> InventoryAccesses => _inventoryAccesses ??= new Repository<InventoryAccess>(_context);
    public IRepository<CustomIdFormat> CustomIdFormats => _customIdFormats ??= new Repository<CustomIdFormat>(_context);
    public IRepository<CustomIdElement> CustomIdElements => _customIdElements ??= new Repository<CustomIdElement>(_context);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
