using Microsoft.EntityFrameworkCore;
using BookLibwithSub.Repo.Entities;

namespace BookLibwithSub.Repo
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanItem> LoanItems { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== USER =====
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(u => u.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            // ===== SUBSCRIPTION PLAN =====
            modelBuilder.Entity<SubscriptionPlan>(entity =>
            {
                entity.Property(p => p.PlanName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(p => p.Price)
                    .HasColumnType("decimal(18,2)");
            });

            // ===== SUBSCRIPTION =====
            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasOne(s => s.User)
                    .WithMany(u => u.Subscriptions)
                    .HasForeignKey(s => s.UserID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.SubscriptionPlan)
                    .WithMany(p => p.Subscriptions)
                    .HasForeignKey(s => s.SubscriptionPlanID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(s => s.Status)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            // ===== BOOK =====
            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(b => b.Title)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(b => b.AuthorName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(b => b.ISBN)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            // ===== LOAN =====
            modelBuilder.Entity<Loan>(entity =>
            {
                entity.HasOne(l => l.Subscription)
                    .WithMany(s => s.Loans)
                    .HasForeignKey(l => l.SubscriptionID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== LOAN ITEM =====
            modelBuilder.Entity<LoanItem>(entity =>
            {
                entity.HasOne(li => li.Loan)
                    .WithMany(l => l.LoanItems)
                    .HasForeignKey(li => li.LoanID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(li => li.Book)
                    .WithMany(b => b.LoanItems)
                    .HasForeignKey(li => li.BookID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== TRANSACTION =====
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasOne(t => t.User)
                    .WithMany(u => u.Transactions)
                    .HasForeignKey(t => t.UserID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Subscription)
                    .WithMany(s => s.Transactions)
                    .HasForeignKey(t => t.SubscriptionID)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.Property(t => t.TransactionType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(t => t.Amount)
                    .HasColumnType("decimal(18,2)");
            });
        }
    }
}
