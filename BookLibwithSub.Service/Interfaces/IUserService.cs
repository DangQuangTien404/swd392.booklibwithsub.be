using System.Threading.Tasks;
using BookLibwithSub.Repo.Entities;
using BookLibwithSub.Service.Models;

namespace BookLibwithSub.Service.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetByIdAsync(int userId);
        Task UpdateProfileAsync(User updated);
        Task<UserProfileDto> GetProfileAsync(int userId);
    }
}
