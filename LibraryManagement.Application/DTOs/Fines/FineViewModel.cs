namespace LibraryManagement.Application.DTOs.Fines;

public class FineViewModel
{
    public int Id { get; set; }
    public int LoanId { get; set; }
    public string ReaderId { get; set; } = string.Empty;
    public string ReaderName { get; set; } = string.Empty;
    public string BookTitle { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidAt { get; set; }
}
