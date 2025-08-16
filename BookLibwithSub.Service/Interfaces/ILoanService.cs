using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookLibwithSub.Service.Interfaces
{
    public interface ILoanService
    {
        Task BorrowAsync(int subscriptionId, IEnumerable<int> bookIds);
        Task AddItemsAsync(int loanId, IEnumerable<int> bookIds);
        Task ReturnAsync(int loanItemId);
    }
}
