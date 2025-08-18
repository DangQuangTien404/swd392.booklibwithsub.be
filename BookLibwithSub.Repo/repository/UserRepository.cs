using System.Threading.Tasks;
using BookLibwithSub.Repo.Entities;
using BookLibwithSub.Repo.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookLibwithSub.Repo
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) => _context = context;

        public Task<User?> GetByIdAsync(int userId) =>
            _context.Users.FirstOrDefaultAsync(u => u.UserID == userId);

        public Task<User?> GetByUsernameAsync(string username) =>
            _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        public Task<User?> GetByEmailAsync(string email) =>
            _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int userId)
        {
            var u = await _context.Users.FirstOrDefaultAsync(x => x.UserID == userId);
            if (u == null) return;
            _context.Users.Remove(u);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTokenAsync(int userId, string? token)
        {
            var u = await _context.Users.FirstOrDefaultAsync(x => x.UserID == userId);
            if (u == null) return;
            u.CurrentToken = token;
            await _context.SaveChangesAsync();
        }

        // ADD THIS:
        public Task<User?> GetByTokenAsync(string token) =>
            _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.CurrentToken == token);
    }
}
