using LibraryManagement.Application.DTOs.Fines;
using LibraryManagement.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Core.Entities;

namespace LibraryManagement.API.Controllers;

[Authorize]
public class FinesController : Controller
{
    private readonly IUnitOfWork _uow;
    private readonly UserManager<ApplicationUser> _userManager;

    public FinesController(IUnitOfWork uow, UserManager<ApplicationUser> userManager)
    {
        _uow = uow;
        _userManager = userManager;
    }

    // GET /Fines — всі несплачені штрафи (Admin/Librarian)
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Index()
    {
        var fines = await _uow.Fines.GetUnpaidAsync();
        return View(fines.Select(MapToViewModel).OrderByDescending(f => f.CreatedAt));
    }

    // GET /Fines/All — всі штрафи (Admin/Librarian)
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> All()
    {
        var fines = await _uow.Fines.GetAllAsync();
        return View(fines.Select(MapToViewModel).OrderByDescending(f => f.CreatedAt));
    }

    // GET /Fines/MyFines — власні штрафи читача
    public async Task<IActionResult> MyFines()
    {
        var userId = _userManager.GetUserId(User)!;
        var fines = await _uow.Fines.GetByReaderIdAsync(userId);
        return View(fines.Select(MapToViewModel).OrderByDescending(f => f.CreatedAt));
    }

    // POST /Fines/Pay/5
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Pay(int id)
    {
        var fine = await _uow.Fines.GetByIdAsync(id);
        if (fine is null) return NotFound();

        fine.IsPaid = true;
        fine.PaidAt = DateTime.UtcNow;
        _uow.Fines.Update(fine);
        await _uow.SaveChangesAsync();

        TempData["Success"] = $"Штраф на суму {fine.Amount:F2} грн сплачено";
        return RedirectToAction(nameof(Index));
    }

    private static FineViewModel MapToViewModel(Core.Entities.Fine f) => new()
    {
        Id = f.Id,
        LoanId = f.LoanId,
        ReaderId = f.ReaderId,
        ReaderName = f.Reader?.FullName ?? "",
        BookTitle = f.Loan?.Book?.Title ?? "",
        Amount = f.Amount,
        Reason = f.Reason,
        CreatedAt = f.CreatedAt,
        IsPaid = f.IsPaid,
        PaidAt = f.PaidAt
    };
}
