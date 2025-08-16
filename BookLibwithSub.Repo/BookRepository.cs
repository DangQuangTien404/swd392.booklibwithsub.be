using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookLibwithSub.Repo.Entities;
using BookLibwithSub.Repo.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookLibwithSub.Repo
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;
        public BookRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<(IEnumerable<Book> Books, int TotalCount)> SearchAsync(string? keyword, int page, int pageSize)
        {
            var query = _context.Books.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(b => b.Title.Contains(keyword) ||
                                         b.AuthorName.Contains(keyword) ||
                                         b.ISBN.Contains(keyword));
            }
            var total = await query.CountAsync();
            var books = await query
                .OrderBy(b => b.BookID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (books, total);
        }
    }
}
