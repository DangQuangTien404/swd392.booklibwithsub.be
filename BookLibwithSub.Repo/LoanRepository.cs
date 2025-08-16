using System;
using System.Linq;
using System.Threading.Tasks;
using BookLibwithSub.Repo.Entities;
using BookLibwithSub.Repo.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookLibwithSub.Repo
{
    public class LoanRepository : ILoanRepository
    {
        private readonly AppDbContext _context;
        public LoanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountLoanItemsAsync(int subscriptionId, DateTime start, DateTime end)
        {
            return await _context.LoanItems
                .Where(li => li.Loan.SubscriptionID == subscriptionId &&
                             li.Loan.LoanDate >= start && li.Loan.LoanDate < end)
                .CountAsync();
        }

        public async Task AddAsync(Loan loan)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();
            foreach (var item in loan.LoanItems)
            {
                var affected = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE Books SET AvailableCopies = AvailableCopies - 1 WHERE BookID = {item.BookID} AND AvailableCopies > 0");
                if (affected == 0)
                    throw new InvalidOperationException($"Book {item.BookID} not available");
            }

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();
        }

        public async Task ReturnAsync(int loanItemId)
        {
            var item = await _context.LoanItems
                .Include(li => li.Book)
                .FirstOrDefaultAsync(li => li.LoanItemID == loanItemId);
            if (item == null)
                throw new InvalidOperationException("Loan item not found");
            if (item.Status == "Returned")
                return;

            item.ReturnedDate = DateTime.UtcNow;
            item.Status = "Returned";
            item.Book.AvailableCopies++;

            await _context.SaveChangesAsync();
        }
    }
}
