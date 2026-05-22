using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Core.Entities;

public class Book
{
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Author { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Genre { get; set; } = string.Empty;

    public int Year { get; set; }

    public int TotalCopies { get; set; }

    public int AvailableCopies { get; set; }

    [MaxLength(20)]
    public string ISBN { get; set; } = string.Empty;

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
