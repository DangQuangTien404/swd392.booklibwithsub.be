using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibwithSub.Repo.Entities;

namespace BookLibwithSub.Repo.Interfaces
{
    public interface IBookRepository
    {
        Task<(IEnumerable<Book> Books, int TotalCount)> SearchAsync(string? keyword, int page, int pageSize);
        Task<Book?> GetByIdAsync(int id);
    }
}
