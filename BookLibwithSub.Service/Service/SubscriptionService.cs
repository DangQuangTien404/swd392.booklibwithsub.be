using System;
using System.Threading.Tasks;
using BookLibwithSub.Repo.Entities;
using BookLibwithSub.Repo.Interfaces;
using BookLibwithSub.Service.Interfaces;

namespace BookLibwithSub.Service.Service
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepo;
        private readonly ISubscriptionPlanRepository _planRepo;
        private readonly IPaymentService _paymentService;
        private readonly IUserRepository _userRepo;

        public SubscriptionService(
            ISubscriptionRepository subscriptionRepo,
            ISubscriptionPlanRepository planRepo,
            IPaymentService paymentService,
            IUserRepository userRepo)
        {
            _subscriptionRepo = subscriptionRepo;
            _planRepo = planRepo;
            _paymentService = paymentService;
            _userRepo = userRepo;
        }

        public async Task<Transaction> PurchaseAsync(int userId, int planId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found. Please login again.");

            var active = await _subscriptionRepo.GetActiveByUserAsync(userId);
            if (active != null)
                throw new InvalidOperationException("User already has an active subscription");

            var plan = await _planRepo.GetByIdAsync(planId);
            if (plan == null)
                throw new InvalidOperationException("Subscription plan not found");

            var start = DateTime.UtcNow;
            var subscription = new Subscription
            {
                UserID = userId,
                SubscriptionPlanID = plan.SubscriptionPlanID,
                StartDate = start,
                EndDate = start.AddDays(plan.DurationDays),
                Status = "Inactive"
            };

            await _subscriptionRepo.AddAsync(subscription);

            return await _paymentService.CreatePendingTransactionAsync(
                userId, subscription.SubscriptionID, plan.Price);
        }

        public async Task<Transaction> RenewAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found. Please login again.");

            var latest = await _subscriptionRepo.GetLatestByUserAsync(userId);
            if (latest == null)
                throw new InvalidOperationException("No subscription to renew");

            var plan = await _planRepo.GetByIdAsync(latest.SubscriptionPlanID);
            if (plan == null)
                throw new InvalidOperationException("Subscription plan not found");

            var start = latest.EndDate > DateTime.UtcNow ? latest.EndDate : DateTime.UtcNow;
            var subscription = new Subscription
            {
                UserID = userId,
                SubscriptionPlanID = plan.SubscriptionPlanID,
                StartDate = start,
                EndDate = start.AddDays(plan.DurationDays),
                Status = "Inactive"
            };

            await _subscriptionRepo.AddAsync(subscription);

            return await _paymentService.CreatePendingTransactionAsync(
                userId, subscription.SubscriptionID, plan.Price);
        }
    }
}
