using System.Threading.Tasks;
using BookLibwithSub.Repo.Entities;

namespace BookLibwithSub.Repo.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<Subscription?> GetByIdWithPlanAsync(int id);
    }
}
