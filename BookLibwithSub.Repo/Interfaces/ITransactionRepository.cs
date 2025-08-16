using System.Threading.Tasks;
using BookLibwithSub.Repo.Entities;

namespace BookLibwithSub.Repo.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
        Task<Transaction?> GetByIdAsync(int id);
        Task UpdateAsync(Transaction transaction);
    }
}
