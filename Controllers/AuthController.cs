using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment_System.Dto_s;
using Recruitment_System.Services;
using System.Security.Claims;

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

            var result = await _authService.LoginAsync(request.Email, request.Password, request.RememberMe);

            if (!result.IsSuccess)
                return BadRequest(result);        

            return Ok(result);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPost("admin-register")]
        public async Task<IActionResult> AdminRegister([FromBody] AdminRegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new RegisterResponse
                {
                    IsSuccess = false,
                    Message = "Invalid request data"
                });

            var result = await _authService.AdminRegisterAsync(request, GetCurrentUserId());

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("candidate-register")]
        public async Task<IActionResult> CandidateRegister([FromBody] CandidateRegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new RegisterResponse
                {
                    IsSuccess = false,
                    Message = "Invalid request data"
                });

            var result = await _authService.CandidateRegisterAsync(request);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }


    }
}
