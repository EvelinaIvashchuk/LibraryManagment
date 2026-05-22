using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagement.Core.Entities;

public class ApplicationUser : IdentityUser
{
    [Required, MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    public bool IsBlocked { get; set; } = false;

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public ICollection<Fine> Fines { get; set; } = new List<Fine>();
}
