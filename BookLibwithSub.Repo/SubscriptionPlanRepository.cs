using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibwithSub.Repo.Entities;
using BookLibwithSub.Repo.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookLibwithSub.Repo
{
    public class SubscriptionPlanRepository : ISubscriptionPlanRepository
    {
        private readonly AppDbContext _context;
        public SubscriptionPlanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SubscriptionPlan>> GetAllAsync()
        {
            return await _context.SubscriptionPlans.ToListAsync();
        }

        public async Task<SubscriptionPlan?> GetByIdAsync(int id)
        {
            return await _context.SubscriptionPlans.FindAsync(id);
        }

        public async Task AddAsync(SubscriptionPlan plan)
        {
            _context.SubscriptionPlans.Add(plan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SubscriptionPlan plan)
        {
            _context.SubscriptionPlans.Update(plan);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(id);
            if (plan != null)
            {
                _context.SubscriptionPlans.Remove(plan);
                await _context.SaveChangesAsync();
            }
        }
    }
}
