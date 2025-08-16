using System.Threading.Tasks;
using BookLibwithSub.Repo.Entities;

namespace BookLibwithSub.Service.Interfaces
{
    public interface ISubscriptionService
    {
        Task<Transaction> PurchaseAsync(int userId, int planId);
        Task<Transaction> RenewAsync(int userId);
    }
}
