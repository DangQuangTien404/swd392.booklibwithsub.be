using System.Threading.Tasks;
using BookLibwithSub.Repo.Entities;

namespace BookLibwithSub.Service.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetByIdAsync(int userId);
        Task UpdateProfileAsync(User updated);
    }
}
