using WholesaleCRM.Domain.Entities;

namespace WholesaleCRM.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Customer> Customers { get; }
    IRepository<Contact> Contacts { get; }
    IRepository<Product> Products { get; }
    IRepository<ProductCategory> ProductCategories { get; }
    IRepository<Deal> Deals { get; }
    IRepository<DealProduct> DealProducts { get; }
    IRepository<Activity> Activities { get; }
    Task<int> SaveChangesAsync();
}
