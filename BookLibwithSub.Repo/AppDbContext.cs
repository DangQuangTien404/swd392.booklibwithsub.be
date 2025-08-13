using BookLibwithSub.Repo.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookLibwithSub.Repo;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
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
    public DbSet<AccountLedger> AccountLedger => Set<AccountLedger>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<BookAuthor>()
            .HasKey(x => new { x.BookId, x.AuthorId });
        b.Entity<BookAuthor>()
            .HasOne(x => x.Book)
            .WithMany(x => x.BookAuthors)
            .HasForeignKey(x => x.BookId);
        b.Entity<BookAuthor>()
            .HasOne(x => x.Author)
            .WithMany(x => x.BookAuthors)
            .HasForeignKey(x => x.AuthorId);

        b.Entity<Inventory>()
            .HasKey(x => x.BookId);
        b.Entity<Inventory>()
            .Property(x => x.RowVersion)
            .IsRowVersion();
        b.Entity<Inventory>()
            .HasOne(x => x.Book)
            .WithOne(x => x.Inventory)
            .HasForeignKey<Inventory>(x => x.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<LoanItem>()
            .HasOne(x => x.Loan)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.LoanId);
        b.Entity<LoanItem>()
            .HasOne(x => x.Book)
            .WithMany(x => x.LoanItems)
            .HasForeignKey(x => x.BookId);

        b.Entity<MonthlyUniqueBorrow>()
            .HasKey(x => new { x.MemberId, x.YearMonth, x.BookId });
        b.Entity<MonthlyUniqueBorrow>()
            .HasOne(x => x.Member)
            .WithMany()
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
        b.Entity<MonthlyUniqueBorrow>()
            .HasOne(x => x.Book)
            .WithMany()
            .HasForeignKey(x => x.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<MemberDailyBorrow>()
            .HasKey(x => new { x.MemberId, x.LocalDate });
        b.Entity<MemberDailyBorrow>()
            .HasOne(x => x.Member)
            .WithMany()
            .HasForeignKey(x => x.MemberId);

        b.Entity<Subscription>()
            .HasOne(x => x.Member)
            .WithMany(x => x.Subscriptions)
            .HasForeignKey(x => x.MemberId);
        b.Entity<Subscription>()
            .HasOne(x => x.Plan)
            .WithMany(x => x.Subscriptions)
            .HasForeignKey(x => x.PlanId);

        b.Entity<AccountLedger>()
            .HasOne(x => x.Member)
            .WithMany(x => x.LedgerEntries)
            .HasForeignKey(x => x.MemberId);
        b.Entity<AccountLedger>()
            .HasOne(x => x.RefLoanItem)
            .WithMany()
            .HasForeignKey(x => x.RefLoanItemId)
            .OnDelete(DeleteBehavior.SetNull);

        b.Entity<Payment>()
            .HasOne(x => x.Member)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.MemberId);
        b.Entity<Payment>()
            .HasOne(x => x.Subscription)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.SubscriptionId);

        b.Entity<SubscriptionPlan>()
            .Property(x => x.Price)
            .HasPrecision(18, 2);

        b.Entity<LoanItem>()
            .Property(x => x.FineAmount)
            .HasPrecision(18, 2);

        b.Entity<AccountLedger>()
            .Property(x => x.Amount)
            .HasPrecision(18, 2);

        b.Entity<Payment>()
            .Property(x => x.Amount)
            .HasPrecision(18, 2);

        b.Entity<Member>()
            .HasIndex(x => x.Email)
            .IsUnique();

        b.Entity<SystemAccount>()
            .HasIndex(x => x.Username)
            .IsUnique();
    }
}
