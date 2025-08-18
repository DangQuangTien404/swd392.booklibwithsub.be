using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using BookLibwithSub.Service.Interfaces;
using BookLibwithSub.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        // ========== AUTH ==========

        /// <summary>Register a new user.</summary>
        [HttpPost("register")]
        [AllowAnonymous]  // no auth required (temporary)
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

        /// <summary>Login and receive a JWT token.</summary>
        [HttpPost("login")]
        [AllowAnonymous]  // no auth required (temporary)
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

        /// <summary>Logout (invalidate current session token).</summary>
        [HttpPost("logout")]
        [AllowAnonymous]  // no auth required (temporary)
        public async Task<IActionResult> Logout()
        {
            // Since we're not requiring auth now, allow optional userId via query/header/body if you want.
            // For a minimal version, do nothing if user is not authenticated.
            try
            {
                var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                if (int.TryParse(userIdClaim, out var userId))
                {
                    await _authService.LogoutAsync(userId);
                }
                // If no authenticated user, just return OK (temporary behavior).
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ========== ACCOUNT MGMT (TEMP: no auth) ==========

        /// <summary>Update an account by id. (TEMP: no auth required)</summary>
        [HttpPut("users/{id:int}")]
        [AllowAnonymous]  // no auth required (temporary)
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _authService.UpdateAccountAsync(id, request);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                // e.g., "User not found" or duplicate username/email
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Delete an account by id. (TEMP: no auth required)</summary>
        [HttpDelete("users/{id:int}")]
        [AllowAnonymous]  // no auth required (temporary)
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                await _authService.DeleteAccountAsync(id);
                return NoContent();
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
