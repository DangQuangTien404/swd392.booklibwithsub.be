using System;
using System.Threading.Tasks;
using BookLibwithSub.Repo.Entities;

namespace BookLibwithSub.Repo.Interfaces
{
    public interface ILoanRepository
    {
        Task<int> CountLoanItemsAsync(int subscriptionId, DateTime start, DateTime end);
        Task AddAsync(Loan loan);
    }
}
