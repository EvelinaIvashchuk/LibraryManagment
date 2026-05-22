using LibraryManagement.Application.DTOs.Loans;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Entities.Enums;
using LibraryManagement.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.API.Controllers;

[Authorize]
public class LoansController : Controller
{
    private readonly IUnitOfWork _uow;
    private readonly UserManager<ApplicationUser> _userManager;

    public LoansController(IUnitOfWork uow, UserManager<ApplicationUser> userManager)
    {
        _uow = uow;
        _userManager = userManager;
    }

    // GET /Loans — активні видачі (Admin/Librarian)
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Index()
    {
        var loans = await _uow.Loans.GetByStatusAsync(LoanStatus.Active);
        return View(loans.Select(MapToViewModel).OrderBy(l => l.DueDate));
    }

    // GET /Loans/Overdue
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Overdue()
    {
        var loans = await _uow.Loans.GetOverdueAsync();
        return View(loans.Select(MapToViewModel).OrderBy(l => l.DueDate));
    }

    // GET /Loans/MyLoans — для читача: власні видачі
    public async Task<IActionResult> MyLoans()
    {
        var userId = _userManager.GetUserId(User)!;
        var loans = await _uow.Loans.GetByReaderIdAsync(userId);
        return View(loans.Select(MapToViewModel).OrderByDescending(l => l.IssuedAt));
    }

    // GET /Loans/Create
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Create()
    {
        await PopulateSelectListsAsync();
        return View(new IssueLoanDto());
    }

    // POST /Loans/Create
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Create(IssueLoanDto dto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateSelectListsAsync();
            return View(dto);
        }

        var book = await _uow.Books.GetByIdAsync(dto.BookId);
        if (book is null || book.AvailableCopies <= 0)
        {
            ModelState.AddModelError("", "Книга недоступна для видачі");
            await PopulateSelectListsAsync();
            return View(dto);
        }

        var reader = await _userManager.FindByIdAsync(dto.ReaderId);
        if (reader is null || reader.IsBlocked)
        {
            ModelState.AddModelError("", "Читач заблокований або не знайдений");
            await PopulateSelectListsAsync();
            return View(dto);
        }

        var loan = new Loan
        {
            BookId = dto.BookId,
            ReaderId = dto.ReaderId,
            IssuedAt = DateTime.UtcNow,
            DueDate = dto.DueDate.ToUniversalTime(),
            Status = LoanStatus.Active
        };

        book.AvailableCopies--;
        _uow.Books.Update(book);
        await _uow.Loans.AddAsync(loan);
        await _uow.SaveChangesAsync();

        TempData["Success"] = $"Книгу «{book.Title}» видано читачу {reader.FullName}";
        return RedirectToAction(nameof(Index));
    }

    // POST /Loans/Return/5
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Return(int id)
    {
        var loan = await _uow.Loans.GetWithDetailsAsync(id);
        if (loan is null) return NotFound();

        loan.ReturnedAt = DateTime.UtcNow;
        loan.Status = LoanStatus.Returned;

        loan.Book.AvailableCopies++;
        _uow.Books.Update(loan.Book);

        // Якщо прострочена — нараховуємо штраф (5 грн / день)
        if (loan.DueDate < DateTime.UtcNow && loan.Fine is null)
        {
            var days = (int)(DateTime.UtcNow - loan.DueDate).TotalDays;
            var fine = new Fine
            {
                LoanId = loan.Id,
                ReaderId = loan.ReaderId,
                Amount = days * 5m,
                Reason = $"Прострочення на {days} {DaysWord(days)}",
                CreatedAt = DateTime.UtcNow
            };
            await _uow.Fines.AddAsync(fine);
        }

        _uow.Loans.Update(loan);
        await _uow.SaveChangesAsync();

        TempData["Success"] = $"Книгу «{loan.Book.Title}» повернено";
        return RedirectToAction(nameof(Index));
    }

    // GET /Loans/Details/5
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Details(int id)
    {
        var loan = await _uow.Loans.GetWithDetailsAsync(id);
        if (loan is null) return NotFound();
        return View(MapToViewModel(loan));
    }

    private async Task PopulateSelectListsAsync()
    {
        var books = await _uow.Books.GetAvailableAsync();
        ViewBag.Books = new SelectList(books, "Id", "Title");

        var readers = await _userManager.GetUsersInRoleAsync("Reader");
        var activeReaders = readers.Where(r => !r.IsBlocked).OrderBy(r => r.FullName);
        ViewBag.Readers = new SelectList(activeReaders, "Id", "FullName");
    }

    private static LoanViewModel MapToViewModel(Loan l) => new()
    {
        Id = l.Id,
        BookId = l.BookId,
        BookTitle = l.Book?.Title ?? "",
        BookAuthor = l.Book?.Author ?? "",
        ReaderId = l.ReaderId,
        ReaderName = l.Reader?.FullName ?? "",
        ReaderEmail = l.Reader?.Email ?? "",
        IssuedAt = l.IssuedAt,
        DueDate = l.DueDate,
        ReturnedAt = l.ReturnedAt,
        Status = l.Status,
        HasFine = l.Fine is not null
    };

    private static string DaysWord(int days) => days switch
    {
        1 => "день",
        2 or 3 or 4 => "дні",
        _ => "днів"
    };
}
