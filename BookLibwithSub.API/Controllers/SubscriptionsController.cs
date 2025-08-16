using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using BookLibwithSub.Service.Constants;
using BookLibwithSub.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibwithSub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionPlanService _planService;
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionsController(ISubscriptionPlanService planService, ISubscriptionService subscriptionService)
        {
            _planService = planService;
            _subscriptionService = subscriptionService;
        }

        [HttpGet("plans")]
        public async Task<IActionResult> GetPlans()
        {
            var plans = await _planService.GetAllAsync();
            return Ok(plans);
        }

        public class PurchaseRequest
        {
            public int PlanId { get; set; }
        }

        [HttpPost("purchase")]
        [Authorize(Roles = Roles.User)]
        public async Task<IActionResult> Purchase([FromBody] PurchaseRequest request)
        {
            var userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? "0");
            var transaction = await _subscriptionService.PurchaseAsync(userId, request.PlanId);
            return Ok(transaction);
        }

        [HttpPost("renew")]
        [Authorize(Roles = Roles.User)]
        public async Task<IActionResult> Renew()
        {
            var userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? "0");
            var transaction = await _subscriptionService.RenewAsync(userId);
            return Ok(transaction);
        }
    }
}
