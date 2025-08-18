using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        public SubscriptionsController(
            ISubscriptionPlanService planService,
            ISubscriptionService subscriptionService)
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

        public class PurchaseRequest { public int PlanId { get; set; } }

        [HttpPost("purchase")]
        [Authorize(Roles = Roles.User)]
        public async Task<IActionResult> Purchase([FromBody] PurchaseRequest request)
        {
            var userIdOpt = GetUserId(User);
            if (userIdOpt == null)
                return Unauthorized(new { message = "Invalid token. Please login again." });

            try
            {
                var transaction = await _subscriptionService.PurchaseAsync(userIdOpt.Value, request.PlanId);
                return Ok(transaction);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("renew")]
        [Authorize(Roles = Roles.User)]
        public async Task<IActionResult> Renew()
        {
            var userIdOpt = GetUserId(User);
            if (userIdOpt == null)
                return Unauthorized(new { message = "Invalid token. Please login again." });

            try
            {
                var transaction = await _subscriptionService.RenewAsync(userIdOpt.Value);
                return Ok(transaction);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ---- helper: reads both "sub" and NameIdentifier ----
        private static int? GetUserId(ClaimsPrincipal user)
        {
            var sub =
                user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(sub, out var id)) return id;
            return null;
            // If you ever include a custom claim, add it here as another fallback.
        }
    }
}
