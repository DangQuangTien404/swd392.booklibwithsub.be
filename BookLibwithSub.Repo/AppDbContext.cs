using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookLibwithSub.Repo.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookLibwithSub.Repo;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Member> Members => Set<Member>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<SystemAccount> SystemAccounts => Set<SystemAccount>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<BookAuthor> BookAuthors => Set<BookAuthor>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<LoanItem> LoanItems => Set<LoanItem>();
    public DbSet<MonthlyUniqueBorrow> MonthlyUniqueBorrows => Set<MonthlyUniqueBorrow>();
    public DbSet<MemberDailyBorrow> MemberDailyBorrows => Set<MemberDailyBorrow>();
    public DbSet<AccountLedger> AccountLedgers => Set<AccountLedger>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // Member
        b.Entity<Member>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
        });

        // SubscriptionPlan
        b.Entity<SubscriptionPlan>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Price).HasPrecision(12, 2);
        });

        // Subscription
        b.Entity<Subscription>(e =>
        {
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
            e.HasOne(x => x.Member)
                .WithMany(m => m.Subscriptions)
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Plan)
                .WithMany(p => p.Subscriptions)
                .HasForeignKey(x => x.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.MemberId, x.Status, x.EndDate }); // common queries
        });

        // SystemAccount (simple auth store for staff)
        b.Entity<SystemAccount>(e =>
        {
            e.Property(x => x.Username).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.Username).IsUnique();
            e.Property(x => x.Password).HasMaxLength(200).IsRequired(); // plain for now, per your ask
            e.Property(x => x.Role).HasMaxLength(50).IsRequired();      // Admin/Librarian
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
        });

        // Author
        b.Entity<Author>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
        });

        // Book
        b.Entity<Book>(e =>
        {
            e.Property(x => x.Title).HasMaxLength(300).IsRequired();
            e.Property(x => x.Isbn).HasMaxLength(20);
            e.HasIndex(x => x.Isbn);
        });

        // BookAuthor (many-to-many)
        b.Entity<BookAuthor>(e =>
        {
            e.HasKey(x => new { x.BookId, x.AuthorId });
            e.HasOne(x => x.Book).WithMany(bk => bk.BookAuthors).HasForeignKey(x => x.BookId);
            e.HasOne(x => x.Author).WithMany(au => au.BookAuthors).HasForeignKey(x => x.AuthorId);
        });

        // Inventory (1:1 with Book)
        b.Entity<Inventory>(e =>
        {
            e.HasKey(x => x.BookId);
            e.Property(x => x.RowVersion).IsRowVersion();
            e.HasOne(x => x.Book).WithOne(bk => bk.Inventory).HasForeignKey<Inventory>(x => x.BookId);
        });

        // Loan
        b.Entity<Loan>(e =>
        {
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
            e.HasOne(x => x.Member).WithMany(m => m.Loans).HasForeignKey(x => x.MemberId);
            e.HasIndex(x => new { x.MemberId, x.Status, x.DueDate });
        });

        // LoanItem
        b.Entity<LoanItem>(e =>
        {
            e.Property(x => x.FineAmount).HasPrecision(12, 2);
            e.HasOne(x => x.Loan).WithMany(l => l.Items).HasForeignKey(x => x.LoanId);
            e.HasOne(x => x.Book).WithMany(bk => bk.LoanItems).HasForeignKey(x => x.BookId);
        });

        // MonthlyUniqueBorrow (quota tracking)
        b.Entity<MonthlyUniqueBorrow>(e =>
        {
            e.HasKey(x => new { x.MemberId, x.YearMonth, x.BookId });
            e.Property(x => x.YearMonth).HasMaxLength(7).IsRequired(); // "YYYY-MM"
            e.HasOne(x => x.Member).WithMany().HasForeignKey(x => x.MemberId);
            e.HasOne(x => x.Book).WithMany().HasForeignKey(x => x.BookId);
        });

        // MemberDailyBorrow (daily concurrent tracking)
        b.Entity<MemberDailyBorrow>(e =>
        {
            e.HasKey(x => new { x.MemberId, x.LocalDate });
            e.Property(x => x.LocalDate).HasColumnType("date"); // store date-only
            e.HasOne(x => x.Member).WithMany().HasForeignKey(x => x.MemberId);
        });

        // AccountLedger
        b.Entity<AccountLedger>(e =>
        {
            e.Property(x => x.EntryType).HasMaxLength(50).IsRequired();
            e.Property(x => x.Amount).HasPrecision(12, 2);
            e.Property(x => x.Currency).HasMaxLength(10).IsRequired();
            e.HasOne(x => x.Member).WithMany(m => m.LedgerEntries).HasForeignKey(x => x.MemberId);
            e.HasOne(x => x.RefLoanItem).WithMany(li => li.LedgerLinks).HasForeignKey(x => x.RefLoanItemId).IsRequired(false);
            e.HasIndex(x => new { x.MemberId, x.EntryDate });
        });

        // Payment
        b.Entity<Payment>(e =>
        {
            e.Property(x => x.Amount).HasPrecision(12, 2);
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
            e.HasOne(x => x.Member).WithMany(m => m.Payments).HasForeignKey(x => x.MemberId);
            e.HasOne(x => x.Subscription).WithMany().HasForeignKey(x => x.SubscriptionId);
            e.HasIndex(x => new { x.MemberId, x.Status, x.CreatedAt });
        });
    }
}

