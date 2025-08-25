using System;
using System.Threading.Tasks;
using BookLibwithSub.Repo.Entities;
using BookLibwithSub.Repo.Interfaces;
using BookLibwithSub.Service.Interfaces;
using BookLibwithSub.Service.Models;

namespace BookLibwithSub.Service.Service
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepo;
        private readonly ISubscriptionPlanRepository _planRepo;
        private readonly IPaymentService _paymentService;

        public SubscriptionService(
            ISubscriptionRepository subscriptionRepo,
            ISubscriptionPlanRepository planRepo,
            IPaymentService paymentService)
        {
            _subscriptionRepo = subscriptionRepo;
            _planRepo = planRepo;
            _paymentService = paymentService;
        }

        public async Task<SubscriptionStatusDto> GetMyStatusAsync(int userId)
        {
            var latest = await _subscriptionRepo.GetLatestByUserAsync(userId);
            if (latest == null)
            {
                return new SubscriptionStatusDto
                {
                    Status = "None",
                    BorrowedToday = 0,
                    BorrowedThisMonth = 0,
                    RemainingToday = 0,
                    RemainingThisMonth = 0
                };
            }

            var plan = await _planRepo.GetByIdAsync(latest.SubscriptionPlanID);

            // If you want real BorrowedToday/BorrowedThisMonth, inject ILoanRepository and compute.
            // For now, keep them 0 so the DTO is consistent and safe.
            var borrowedToday = 0;
            var borrowedThisMonth = 0;

            int? maxPerDay = plan?.MaxPerDay;
            int? maxPerMonth = plan?.MaxPerMonth;

            return new SubscriptionStatusDto
            {
                SubscriptionId = latest.SubscriptionID,
                PlanName = plan?.PlanName,
                DurationDays = plan?.DurationDays,
                Price = plan?.Price,
                StartDate = latest.StartDate,
                EndDate = latest.EndDate,
                Status = latest.Status,
                MaxPerDay = maxPerDay,
                MaxPerMonth = maxPerMonth,
                BorrowedToday = borrowedToday,
                BorrowedThisMonth = borrowedThisMonth,
                RemainingToday = maxPerDay.HasValue ? Math.Max(0, maxPerDay.Value - borrowedToday) : 0,
                RemainingThisMonth = maxPerMonth.HasValue ? Math.Max(0, maxPerMonth.Value - borrowedThisMonth) : 0
            };
        }

        public async Task<Transaction> PurchaseAsync(int userId, int planId)
        {
            var plan = await _planRepo.GetByIdAsync(planId)
                ?? throw new InvalidOperationException("Subscription plan not found");

            var nowUtc = DateTime.UtcNow;

            // If user already has an active subscription, only allow if this is an upgrade (longer DurationDays)
            var active = await _subscriptionRepo.GetActiveByUserAsync(userId);
            if (active != null)
            {
                var activeWithPlan = await _subscriptionRepo.GetByIdWithPlanAsync(active.SubscriptionID) ?? active;
                var activeDays = activeWithPlan.SubscriptionPlan?.DurationDays ?? 0;

                if (plan.DurationDays <= activeDays)
                    throw new InvalidOperationException("You already have an active subscription with equal or longer duration.");

                // Deactivate/close the old active subscription immediately
                active.Status = "Inactive";
                active.EndDate = nowUtc;
                await _subscriptionRepo.UpdateAsync(active);
            }

            // Create and immediately activate the new subscription
            var newSub = new Subscription
            {
                UserID = userId,
                SubscriptionPlanID = plan.SubscriptionPlanID,
                StartDate = nowUtc,
                EndDate = nowUtc.AddDays(plan.DurationDays),
                Status = "Active"
            };
            await _subscriptionRepo.AddAsync(newSub);

            // Create a pending transaction tied to this new subscription
            var tx = await _paymentService.CreatePendingTransactionAsync(userId, newSub.SubscriptionID, plan.Price);
            return tx;
        }

        public async Task<Transaction> RenewAsync(int userId)
        {
            var latest = await _subscriptionRepo.GetLatestByUserAsync(userId)
                ?? throw new InvalidOperationException("No subscription found to renew");

            var subWithPlan = await _subscriptionRepo.GetByIdWithPlanAsync(latest.SubscriptionID)
                ?? throw new InvalidOperationException("Subscription not found");

            var plan = subWithPlan.SubscriptionPlan
                ?? throw new InvalidOperationException("Subscription plan not found");

            var nowUtc = DateTime.UtcNow;
            latest.StartDate = nowUtc;
            latest.EndDate = nowUtc.AddDays(plan.DurationDays);
            latest.Status = "Active";
            await _subscriptionRepo.UpdateAsync(latest);

            var tx = await _paymentService.CreatePendingTransactionAsync(userId, latest.SubscriptionID, plan.Price);
            return tx;
        }
    }
}
