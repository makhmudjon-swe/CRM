using System.ComponentModel.DataAnnotations;

namespace WholesaleCRM.Application.Requests;

public class ProductRequest
{
    [Required(ErrorMessage = "Mahsulot nomi majburiy")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? SKU { get; set; }

    public string? Description { get; set; }

    [Required(ErrorMessage = "Kategoriya majburiy")]
    public int ProductCategoryId { get; set; }

    [Required(ErrorMessage = "Narx majburiy")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Narx 0 dan katta bo'lishi kerak")]
    public decimal UnitPrice { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Miqdor manfiy bo'lishi mumkin emas")]
    public int StockQuantity { get; set; }

    public bool IsActive { get; set; } = true;
}
