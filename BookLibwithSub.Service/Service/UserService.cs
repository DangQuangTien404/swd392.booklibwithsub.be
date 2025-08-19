using System.Threading.Tasks;
using BookLibwithSub.Repo.Entities;
using BookLibwithSub.Repo.Interfaces;
using BookLibwithSub.Service.Interfaces;
using BookLibwithSub.Service.Models;

namespace BookLibwithSub.Service.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;

        public UserService(
            IUserRepository userRepository,
            ISubscriptionRepository subscriptionRepository,  
            ISubscriptionPlanRepository subscriptionPlanRepository)  
        {
            _userRepository = userRepository;
            _subscriptionRepository = subscriptionRepository;  
            _subscriptionPlanRepository = subscriptionPlanRepository; 
        }

        public Task<User?> GetByIdAsync(int userId)
            => _userRepository.GetByIdAsync(userId);

        public async Task UpdateProfileAsync(User updated)
            => await _userRepository.UpdateAsync(updated);

        public async Task<UserProfileDto> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

            var latest = await _subscriptionRepository.GetLatestByUserAsync(userId);

            CurrentSubscriptionDto? current = null;
            if (latest != null)
            {
                var plan = await _subscriptionPlanRepository.GetByIdAsync(latest.SubscriptionPlanID);
                current = new CurrentSubscriptionDto
                {
                    SubscriptionId = latest.SubscriptionID,
                    PlanName = plan?.PlanName ?? "Unknown",
                    Status = latest.Status,
                    EndDate = latest.EndDate
                };
            }

            return new UserProfileDto
            {
                UserId = user.UserID,
                Username = user.Username,
                Role = user.Role,
                CurrentSubscription = current
            };
        }
    }
}
