using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Entities.Enums;

namespace LibraryManagement.Core.Interfaces;

public interface ILoanRepository : IRepository<Loan>
{
    Task<IEnumerable<Loan>> GetByReaderIdAsync(string readerId);
    Task<IEnumerable<Loan>> GetByStatusAsync(LoanStatus status);
    Task<IEnumerable<Loan>> GetOverdueAsync();
    Task<Loan?> GetWithDetailsAsync(int id);
}
