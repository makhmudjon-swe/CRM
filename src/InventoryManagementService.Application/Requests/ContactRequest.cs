using System.ComponentModel.DataAnnotations;

namespace WholesaleCRM.Application.Requests;

public class ContactRequest
{
    [Required(ErrorMessage = "Ism majburiy")]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Familiya majburiy")]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Noto'g'ri email format")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Noto'g'ri telefon format")]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? Position { get; set; }

    [Required(ErrorMessage = "Kompaniya majburiy")]
    public int CustomerId { get; set; }

    public bool IsPrimary { get; set; }
}
