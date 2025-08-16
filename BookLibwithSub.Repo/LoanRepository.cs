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
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
        }
    }
}
