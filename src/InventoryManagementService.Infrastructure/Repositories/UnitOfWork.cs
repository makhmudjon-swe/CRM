using WholesaleCRM.Domain.Entities;
using WholesaleCRM.Domain.Interfaces;
using WholesaleCRM.Infrastructure.Data;

namespace WholesaleCRM.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private IRepository<Customer>? _customers;
    private IRepository<Contact>? _contacts;
    private IRepository<Product>? _products;
    private IRepository<ProductCategory>? _productCategories;
    private IRepository<Deal>? _deals;
    private IRepository<DealProduct>? _dealProducts;
    private IRepository<Activity>? _activities;

    public UnitOfWork(AppDbContext context) => _context = context;

    public IRepository<Customer> Customers => _customers ??= new Repository<Customer>(_context);
    public IRepository<Contact> Contacts => _contacts ??= new Repository<Contact>(_context);
    public IRepository<Product> Products => _products ??= new Repository<Product>(_context);
    public IRepository<ProductCategory> ProductCategories => _productCategories ??= new Repository<ProductCategory>(_context);
    public IRepository<Deal> Deals => _deals ??= new Repository<Deal>(_context);
    public IRepository<DealProduct> DealProducts => _dealProducts ??= new Repository<DealProduct>(_context);
    public IRepository<Activity> Activities => _activities ??= new Repository<Activity>(_context);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
