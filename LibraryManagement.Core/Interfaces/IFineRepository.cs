using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Interfaces;

public interface IFineRepository : IRepository<Fine>
{
    Task<IEnumerable<Fine>> GetByReaderIdAsync(string readerId);
    Task<IEnumerable<Fine>> GetUnpaidAsync();
    Task<decimal> GetTotalUnpaidByReaderAsync(string readerId);
}
