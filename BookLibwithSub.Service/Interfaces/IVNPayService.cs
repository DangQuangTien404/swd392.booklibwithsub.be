using System.Threading.Tasks;
using BookLibwithSub.Service.Models;

namespace BookLibwithSub.Service.Interfaces
{
    public interface IVNPayService
    {
        Task<VNPayCreateOrderResult> CreateOrderAsync(int transactionId, int userId);
        Task<VNPayCallbackResult> HandleCallbackAsync(VNPayCallbackRequest request);
    }
}
