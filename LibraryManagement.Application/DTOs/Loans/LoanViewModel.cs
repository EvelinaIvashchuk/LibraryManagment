using LibraryManagement.Core.Entities.Enums;

namespace LibraryManagement.Application.DTOs.Loans;

public class LoanViewModel
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string BookAuthor { get; set; } = string.Empty;
    public string ReaderId { get; set; } = string.Empty;
    public string ReaderName { get; set; } = string.Empty;
    public string ReaderEmail { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public LoanStatus Status { get; set; }
    public bool HasFine { get; set; }

    public bool IsOverdue => Status == LoanStatus.Active && DueDate < DateTime.UtcNow;
    public int DaysOverdue => IsOverdue ? (int)(DateTime.UtcNow - DueDate).TotalDays : 0;
}
