// LoanService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookLibwithSub.Repo.Entities;
using BookLibwithSub.Repo.Interfaces;
using BookLibwithSub.Service.Interfaces;

namespace BookLibwithSub.Service.Service
{
    public class LoanService : ILoanService
    {
        private readonly ISubscriptionRepository _subscriptionRepo;
        private readonly ILoanRepository _loanRepo;
        private readonly IPaymentService _paymentService;

        public LoanService(ISubscriptionRepository subscriptionRepo, ILoanRepository loanRepo, IPaymentService paymentService)
        {
            _subscriptionRepo = subscriptionRepo;
            _loanRepo = loanRepo;
            _paymentService = paymentService;
        }

        public async Task<Loan> BorrowAsync(int subscriptionId, IEnumerable<int> bookIds)
        {
            var ids = bookIds?.Distinct().ToList() ?? new List<int>();
            if (ids.Count == 0) throw new InvalidOperationException("No book ids provided");

            var subscription = await _subscriptionRepo.GetByIdWithPlanAsync(subscriptionId)
                ?? throw new InvalidOperationException("Subscription not found");

            if (subscription.Status != "Active" || subscription.StartDate > DateTime.UtcNow || subscription.EndDate < DateTime.UtcNow)
                throw new InvalidOperationException("Subscription is not active or out of date range");

            var unreturned = await _loanRepo.CountUnreturnedItemsAsync(subscription.UserID);
            if (unreturned > 0) throw new InvalidOperationException("Please return all borrowed items before borrowing more");

            var plan = subscription.SubscriptionPlan ?? throw new InvalidOperationException("Subscription plan missing");

            var now = DateTime.UtcNow;
            var dayStart = now.Date;
            var dayEnd = dayStart.AddDays(1);
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var monthEnd = monthStart.AddMonths(1);

            var alreadyToday = await _loanRepo.CountLoanItemsAsync(subscriptionId, dayStart, dayEnd);
            var alreadyMonth = await _loanRepo.CountLoanItemsAsync(subscriptionId, monthStart, monthEnd);
            var requested = ids.Count;

            if (alreadyToday + requested > plan.MaxPerDay)
                throw new InvalidOperationException($"Daily borrowing limit exceeded. requested={requested}, alreadyToday={alreadyToday}, maxPerDay={plan.MaxPerDay}");

            if (alreadyMonth + requested > plan.MaxPerMonth)
                throw new InvalidOperationException($"Monthly borrowing limit exceeded. requested={requested}, alreadyMonth={alreadyMonth}, maxPerMonth={plan.MaxPerMonth}");

            var loan = new Loan
            {
                SubscriptionID = subscriptionId,
                LoanDate = now,
                Status = "Borrowed",
                LoanItems = ids.Select(id => new LoanItem
                {
                    BookID = id,
                    DueDate = now.AddDays(14),
                    Status = "Borrowed"
                }).ToList()
            };

            await _loanRepo.AddAsync(loan);

            var loaded = await _loanRepo.GetByIdAsync(loan.LoanID)
                         ?? throw new InvalidOperationException("Created loan not found");
            return loaded;
        }

        public async Task<Loan> AddItemsAsync(int loanId, IEnumerable<int> bookIds)
        {
            var ids = bookIds?.Distinct().ToList() ?? new List<int>();
            if (ids.Count == 0) throw new InvalidOperationException("No book ids provided");

            var loan = await _loanRepo.GetByIdAsync(loanId) ?? throw new InvalidOperationException("Loan not found");

            var subscription = loan.Subscription ?? throw new InvalidOperationException("Subscription not found");
            if (subscription.Status != "Active" || subscription.StartDate > DateTime.UtcNow || subscription.EndDate < DateTime.UtcNow)
                throw new InvalidOperationException("Subscription is not active or out of date range");

            var plan = subscription.SubscriptionPlan ?? throw new InvalidOperationException("Subscription plan missing");

            var now = DateTime.UtcNow;
            var dayStart = now.Date;
            var dayEnd = dayStart.AddDays(1);
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var monthEnd = monthStart.AddMonths(1);

            var existingBorrowed = loan.LoanItems
                .Where(i => i.Status == "Borrowed")
                .Select(i => i.BookID)
                .ToHashSet();

            var toBorrow = ids.Where(id => !existingBorrowed.Contains(id)).ToList();
            if (toBorrow.Count == 0) return loan;

            var alreadyToday = await _loanRepo.CountLoanItemsAsync(subscription.SubscriptionID, dayStart, dayEnd);
            var alreadyMonth = await _loanRepo.CountLoanItemsAsync(subscription.SubscriptionID, monthStart, monthEnd);
            var requested = toBorrow.Count;

            if (alreadyToday + requested > plan.MaxPerDay)
                throw new InvalidOperationException($"Daily borrowing limit exceeded. requested={requested}, alreadyToday={alreadyToday}, maxPerDay={plan.MaxPerDay}");

            if (alreadyMonth + requested > plan.MaxPerMonth)
                throw new InvalidOperationException($"Monthly borrowing limit exceeded. requested={requested}, alreadyMonth={alreadyMonth}, maxPerMonth={plan.MaxPerMonth}");

            var items = toBorrow.Select(id => new LoanItem
            {
                BookID = id,
                DueDate = now.AddDays(14),
                Status = "Borrowed"
            }).ToList();

            await _loanRepo.AddItemsAsync(loan, items);

            var loaded = await _loanRepo.GetByIdAsync(loan.LoanID)
                         ?? throw new InvalidOperationException("Updated loan not found");
            return loaded;
        }

        public async Task<LoanItem> ReturnAsync(int loanItemId)
        {
            var item = await _loanRepo.ReturnAsync(loanItemId);

            if (item.ReturnedDate.HasValue && item.ReturnedDate.Value > item.DueDate)
            {
                var daysLate = (item.ReturnedDate.Value.Date - item.DueDate.Date).Days;
                if (daysLate > 0)
                {
                    var fineAmount = daysLate * 1m;
                    var userId = item.Loan?.Subscription?.UserID ?? 0;
                    if (userId != 0) await _paymentService.RecordFineAsync(userId, item.LoanItemID, fineAmount);
                }
            }

            return item;
        }

        public async Task<Loan?> GetLoanAsync(int loanId, int userId)
        {
            var loan = await _loanRepo.GetByIdAsync(loanId);
            if (loan == null || loan.Subscription?.UserID != userId) return null;
            return loan;
        }

        public async Task<Loan> ExtendLoanAsync(int loanId, int userId, DateTime? newDueDate, int? daysToExtend)
        {
            if (!newDueDate.HasValue && !daysToExtend.HasValue)
                throw new ArgumentException("Either new due date or days to extend must be provided");

            var loan = await _loanRepo.GetByIdAsync(loanId);
            if (loan == null || loan.Subscription?.UserID != userId)
                throw new InvalidOperationException("Loan not found");

            await _loanRepo.ExtendLoanAsync(loan, newDueDate, daysToExtend);

            var loaded = await _loanRepo.GetByIdAsync(loan.LoanID)
                         ?? throw new InvalidOperationException("Updated loan not found");
            return loaded;
        }

        public Task<IEnumerable<Loan>> GetLoanHistoryAsync(int userId)
            => _loanRepo.GetLoansByUserAsync(userId).ContinueWith(t => (IEnumerable<Loan>)t.Result);

        public Task<IEnumerable<Loan>> GetActiveLoansAsync(int userId)
            => _loanRepo.GetActiveLoansByUserAsync(userId).ContinueWith(t => (IEnumerable<Loan>)t.Result);

        public Task<int> DeleteHistoryAsync(int userId)
            => _loanRepo.DeleteHistoryAsync(userId);
    }
}
