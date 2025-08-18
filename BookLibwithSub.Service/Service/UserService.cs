using System.Threading.Tasks;
using BookLibwithSub.Repo.Entities;
using BookLibwithSub.Repo.Interfaces;
using BookLibwithSub.Service.Interfaces;

namespace BookLibwithSub.Service.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<User?> GetByIdAsync(int userId)
        {
            return _userRepository.GetByIdAsync(userId);
        }

        public async Task UpdateProfileAsync(User updated)
        {
            await _userRepository.UpdateAsync(updated);
        }
    }
}
