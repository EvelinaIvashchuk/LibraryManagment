namespace LibraryManagement.Core.Entities;

public class Fine
{
    public int Id { get; set; }

    public int LoanId { get; set; }
    public Loan Loan { get; set; } = null!;

    public string ReaderId { get; set; } = string.Empty;
    public ApplicationUser Reader { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Reason { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsPaid { get; set; } = false;

    public DateTime? PaidAt { get; set; }
}
