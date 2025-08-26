using BookLibwithSub.Service.Constants;
using BookLibwithSub.Service.Interfaces;
using BookLibwithSub.Service.Models;
using BookLibwithSub.Service.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookLibwithSub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous] 
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _authService.RegisterAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous] 
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var token = await _authService.LoginAsync(request);
                if (token == null) return Unauthorized();
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                if (int.TryParse(userIdClaim, out var userId))
                {
                    await _authService.LogoutAsync(userId);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpDelete("users/{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                           ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();

            if (userId != id && !User.IsInRole(Roles.Admin)) return Forbid();

            try
            {
                await _authService.DeleteAccountAsync(id);
                return Ok(new { message = $"User with ID {id} deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
