using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment_System.Dto_s.CandidateDtos;
using Recruitment_System.Services;
using System.Security.Claims;

namespace Recruitment_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Recruiter,HR,Admin")]
    public class CandidateController : ControllerBase
    {
        private readonly CandidateService _candidateService;

        public CandidateController(CandidateService candidateService)
        {
            _candidateService = candidateService;
        }
        [HttpPost("manual")]
        public async Task<IActionResult> CreateCandidate([FromBody] CreateCandidateRequest request)
        {
            if(!ModelState.IsValid) 
                return BadRequest(new
                {
                    IsSuccess = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });

            try
            {
                var userId = GetCurrentUserId();
                var result = await _candidateService.CreateCandidateManualAsync(request, userId);
                return Ok(new { IsSuccess = true, Message = "Candidate created successfully", Data = result });

            }
            catch (Exception e)
            {
                return BadRequest(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpPost ("upload-resume")]
        [Authorize(Roles = "Recruiter,Admin,HR")]
        public async Task<IActionResult> UploadResume([FromForm] ResumeUploadRequest request, [FromQuery] int jobId)
        {
            try
            {
                var result = await _candidateService.UploadResumeAsync(request.File, GetCurrentUserId(), jobId);
                return Ok(new { IsSuccess = true, Message = "Resume processed successfully", Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Recruiter,HR,Admin")]
        public async Task<IActionResult> GetAllCandidates()
        {
            try
            {
                var result = await _candidateService.GetAllCandidatesAsync();
                return Ok(new
                {
                    IsSuccess = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }


        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }
    }
}
