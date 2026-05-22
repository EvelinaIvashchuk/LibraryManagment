using LibraryManagement.Core.Entities.Enums;

namespace LibraryManagement.Core.Entities;

public class Loan
{
    public int Id { get; set; }

    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    public string ReaderId { get; set; } = string.Empty;
    public ApplicationUser Reader { get; set; } = null!;

    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    public DateTime DueDate { get; set; }

    public DateTime? ReturnedAt { get; set; }

    public LoanStatus Status { get; set; } = LoanStatus.Active;

    public Fine? Fine { get; set; }
}
