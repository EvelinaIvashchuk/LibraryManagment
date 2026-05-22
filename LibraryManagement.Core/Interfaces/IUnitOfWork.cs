namespace LibraryManagement.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IBookRepository Books { get; }
    ILoanRepository Loans { get; }
    IFineRepository Fines { get; }

    Task<int> SaveChangesAsync();
}
