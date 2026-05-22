using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Entities.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Data;

public class LibraryDbContext : IdentityDbContext<ApplicationUser>
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<Fine> Fines => Set<Fine>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Book
        builder.Entity<Book>(e =>
        {
            e.HasKey(b => b.Id);
            e.Property(b => b.Title).IsRequired().HasMaxLength(300);
            e.Property(b => b.Author).IsRequired().HasMaxLength(200);
            e.Property(b => b.Genre).IsRequired().HasMaxLength(100);
            e.Property(b => b.ISBN).HasMaxLength(20);
            e.HasIndex(b => b.ISBN).IsUnique();
        });

        // ApplicationUser
        builder.Entity<ApplicationUser>(e =>
        {
            e.Property(u => u.FullName).IsRequired().HasMaxLength(200);
            e.Property(u => u.Phone).HasMaxLength(20);
            e.Property(u => u.Address).HasMaxLength(500);
        });

        // Loan
        builder.Entity<Loan>(e =>
        {
            e.HasKey(l => l.Id);
            e.Property(l => l.Status).HasConversion<string>();

            e.HasOne(l => l.Book)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(l => l.Reader)
                .WithMany(u => u.Loans)
                .HasForeignKey(l => l.ReaderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Fine — один штраф на одну видачу
        builder.Entity<Fine>(e =>
        {
            e.HasKey(f => f.Id);
            e.Property(f => f.Amount).HasColumnType("decimal(10,2)");
            e.Property(f => f.Reason).IsRequired().HasMaxLength(500);

            e.HasOne(f => f.Loan)
                .WithOne(l => l.Fine)
                .HasForeignKey<Fine>(f => f.LoanId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(f => f.Reader)
                .WithMany(u => u.Fines)
                .HasForeignKey(f => f.ReaderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Перейменовуємо таблиці Identity для чистоти
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRole>().ToTable("Roles");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>().ToTable("RoleClaims");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>().ToTable("UserTokens");
    }
}
