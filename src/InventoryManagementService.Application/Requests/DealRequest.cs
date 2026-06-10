using System.ComponentModel.DataAnnotations;

namespace WholesaleCRM.Application.Requests;

public class DealRequest
{
    [Required(ErrorMessage = "Bitim nomi majburiy")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mijoz majburiy")]
    public int CustomerId { get; set; }

    public string? AssignedToId { get; set; }

    public int Status { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Summa manfiy bo'lishi mumkin emas")]
    public decimal TotalAmount { get; set; }

    public DateTime? ExpectedCloseDate { get; set; }

    public string? Notes { get; set; }
}
