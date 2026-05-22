namespace LibraryManagement.Application.DTOs.Readers;

public class ReaderViewModel
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime RegisteredAt { get; set; }
    public bool IsBlocked { get; set; }
    public int ActiveLoans { get; set; }
    public decimal UnpaidFines { get; set; }
}
