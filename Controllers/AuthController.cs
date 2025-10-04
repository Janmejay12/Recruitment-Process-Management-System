using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment_System.Dto_s;
using Recruitment_System.Services;

namespace Recruitment_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new LoginResponse
                {
                    IsSuccess = false,
                    Message = "Invalid request data"
                });

            var (ok, message, token, fullName, email, roles) =
                await _authService.LoginAsync(request.Email, request.Password, request.RememberMe);

            if (!ok)
                return Unauthorized(new LoginResponse
                {
                    IsSuccess = false,
                    Message = message
                });

            var expiryHours = request.RememberMe ? 168 : 24;

            return Ok(new LoginResponse
            {
                IsSuccess = true,
                Message = message,
                Token = token,
                FullName = fullName,
                Email = email,
                Roles = roles.ToList(),
                ExpiresAt = DateTime.UtcNow.AddHours(expiryHours)
            });
        }
    }
}
