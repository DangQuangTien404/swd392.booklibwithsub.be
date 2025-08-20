using System.Threading.Tasks;
using BookLibwithSub.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibwithSub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public class WebhookRequest
        {
            public int TransactionId { get; set; }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] WebhookRequest request, [FromHeader] string signature)
        {
            if (!ValidateSignature(request, signature)) return Unauthorized();
            await _paymentService.MarkTransactionPaidAsync(request.TransactionId);
            return Ok();
        }
        private bool ValidateSignature(object request, string signature)
        {
             return signature == "my-secret";
        }

    }
}
