using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookLibwithSub.Service.Interfaces
{
    public interface ILoanService
    {
        Task BorrowAsync(int subscriptionId, IEnumerable<int> bookIds);
    }
}
