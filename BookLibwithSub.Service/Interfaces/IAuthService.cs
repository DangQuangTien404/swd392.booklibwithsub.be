using System.Threading.Tasks;
using BookLibwithSub.Service.Models;

namespace BookLibwithSub.Service.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest request);
        Task<string?> LoginAsync(LoginRequest request);
        Task LogoutAsync(int userId);
    }
}
