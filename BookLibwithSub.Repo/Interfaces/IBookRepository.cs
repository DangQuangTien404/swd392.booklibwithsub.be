using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibwithSub.Repo.Entities;

namespace BookLibwithSub.Repo.Interfaces
{
    public interface IBookRepository
    {
        // Paging + keyword search (title/author/isbn)
        Task<(IEnumerable<Book> items, int total)> SearchAsync(string? q, int page, int pageSize);

        // Exact title match (used by /api/books/by-title)
        Task<IEnumerable<Book>> GetByExactTitleAsync(string title);

        // Single
        Task<Book?> GetByIdAsync(int id);

        // Create / Update / Delete
        Task<Book> AddAsync(Book entity);
        Task UpdateAsync(Book entity);
        Task DeleteAsync(int id);

        // Helpers
        Task<bool> IsIsbnTakenAsync(string isbn, int? exceptBookId = null);
    }
}