namespace WholesaleCRM.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public string? Description { get; set; }
    public int ProductCategoryId { get; set; }
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ProductCategory Category { get; set; } = null!;
    public ICollection<DealProduct> DealProducts { get; set; } = new List<DealProduct>();
}
