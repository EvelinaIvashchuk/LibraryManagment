using LibraryManagement.Application.DTOs.Loans;

namespace LibraryManagement.Application.DTOs;

public class DashboardViewModel
{
    public int TotalBooks { get; set; }
    public int TotalReaders { get; set; }
    public int ActiveLoans { get; set; }
    public int OverdueLoans { get; set; }
    public decimal UnpaidFines { get; set; }
    public IEnumerable<LoanViewModel> RecentLoans { get; set; } = [];
}
