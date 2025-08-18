using System.Threading.Tasks;
using BookLibwithSub.Service.Models;

namespace BookLibwithSub.Service.Interfaces
{
    public interface IAuthService
    {
        // Auth
        Task RegisterAsync(RegisterRequest request);
        Task<string?> LoginAsync(LoginRequest request);
        Task LogoutAsync(int userId);

        // Account management
        Task UpdateAccountAsync(int userId, UpdateUserRequest request);
        Task DeleteAccountAsync(int userId);
    }
}
