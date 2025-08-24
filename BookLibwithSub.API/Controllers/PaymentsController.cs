using System.Threading.Tasks;
using BookLibwithSub.Service.Interfaces;
using BookLibwithSub.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BookLibwithSub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IVNPayService _vnPayService;
        private readonly VNPayOptions _vnPayOptions;

        public PaymentsController(IPaymentService paymentService, IVNPayService vnPayService, IOptions<VNPayOptions> vnPayOptions)
        {
            _paymentService = paymentService;
            _vnPayService = vnPayService;
            _vnPayOptions = vnPayOptions.Value;
        }

        [HttpPost("vnpay/create-order/{transactionId}")]
        [Authorize]
        public async Task<IActionResult> CreateVNPayOrder([FromRoute] int transactionId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            var result = await _vnPayService.CreateOrderAsync(transactionId, userId.Value);
            return Ok(result);
        }

        [HttpPost("vnpay/callback")]
        [AllowAnonymous]
        public async Task<IActionResult> VNPayCallback([FromForm] VNPayCallbackRequest request)
        {
            var result = await _vnPayService.HandleCallbackAsync(request);
            if (!result.Success) return Ok(new { return_code = -1, return_message = result.Message });
            return Ok(new { return_code = 1, return_message = "success" });
        }

        [HttpGet("vnpay/return")]
        [AllowAnonymous]
        public async Task<IActionResult> VNPayReturn([FromQuery] VNPayCallbackRequest request)
        {
            var result = await _vnPayService.HandleCallbackAsync(request);
            if (!result.Success)
            {
                return Redirect($"{_vnPayOptions.ReturnUrl}?status=failed&message={Uri.EscapeDataString(result.Message)}");
            }
            
            return Redirect($"{_vnPayOptions.ReturnUrl}?status=success&transactionId={result.TransactionId}");
        }

        private int? GetUserId()
        {
            var sub = User?.FindFirst("sub")?.Value ?? User?.FindFirst("nameid")?.Value;
            return int.TryParse(sub, out var id) ? id : (int?)null;
        }
    }
}
