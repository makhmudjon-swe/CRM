namespace WholesaleCRM.Domain.Entities;

public class DealProduct
{
    public int Id { get; set; }
    public int DealId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }

    public Deal Deal { get; set; } = null!;
    public Product Product { get; set; } = null!;

    public decimal TotalPrice => Quantity * UnitPrice * (1 - Discount / 100);
}
