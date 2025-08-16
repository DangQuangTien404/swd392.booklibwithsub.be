using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibwithSub.Repo.Entities;

namespace BookLibwithSub.Repo.Interfaces
{
    public interface ILoanRepository
    {
        Task<int> CountLoanItemsAsync(int subscriptionId, DateTime start, DateTime end);
        Task AddAsync(Loan loan);
        Task<Loan?> GetByIdAsync(int loanId);
        Task AddItemsAsync(Loan loan, IEnumerable<LoanItem> items);
        Task ReturnAsync(int loanItemId);
    }
}
