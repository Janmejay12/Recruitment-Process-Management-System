using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment_System.Dto_s.ScreeningDtos;
using Recruitment_System.Services;
using System.Security.Claims;

namespace Recruitment_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ScreeningController : ControllerBase
    {
        private readonly ScreeningService _screeningService;

        public ScreeningController(ScreeningService screeningService)
        {
            _screeningService = screeningService;
        }

        
        [HttpPost("create")]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Invalid request data" });

            try
            {
                var review = await _screeningService.CreateReviewAsync(
                    request.CandidateId,
                    request.JobId);

                return Ok(new
                {
                    IsSuccess = true,
                    Message = "Candidate review created",
                    Data = new { review.ReviewId }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        
        [HttpPut("{id}/assign-reviewer")]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> AssignReviewer(
            [FromRoute] int id,
            [FromBody] AssignReviewerRequest request)
        {
            try
            {
                await _screeningService.AssignReviewerAsync(id, request.ReviewerUserId);
                return Ok(new { IsSuccess = true, Message = "Reviewer assigned successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        
        [HttpPut("{id}/skills")]
        [Authorize(Roles = "Reviewer,Recruiter")]
        public async Task<IActionResult> SaveSkillEvaluation(
            [FromRoute] int id,
            [FromBody] SkillEvaluationRequest request)
        {
            try
            {
                await _screeningService.SaveSkillEvaluationAsync(
                    id,
                    request.SkillId,
                    request.YearsExperience,
                    GetCurrentUserId());

                return Ok(new { IsSuccess = true, Message = "Skill evaluation saved" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        
        [HttpPost("{id}/comment")]
        [Authorize(Roles = "Reviewer,Recruiter,Interviewer")]
        public async Task<IActionResult> AddComment(
            [FromRoute] int id,
            [FromBody] AddCommentRequest request)
        {
            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";

                await _screeningService.AddCommentAsync(
                    id,
                    request.CommentText,
                    GetCurrentUserId(),
                    role);

                return Ok(new { IsSuccess = true, Message = "Comment added" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        
        [HttpPut("{id}/shortlist")]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> Shortlist(
            [FromRoute] int id,
            [FromBody] ShortlistRequest request)
        {
            try
            {
                await _screeningService.ShortlistAsync(id, request.InterviewerUserId);
                return Ok(new { IsSuccess = true, Message = "Candidate shortlisted for interview" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

       
        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Reviewer,Recruiter,Admin")]
        public async Task<IActionResult> Reject([FromRoute] int id)
        {
            try
            {
                await _screeningService.RejectAsync(id);
                return Ok(new { IsSuccess = true, Message = "Candidate rejected" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        
        [HttpGet("history")]
        [Authorize(Roles = "Recruiter,Reviewer,Interviewer")]
        public async Task<IActionResult> CheckHistory([FromQuery] string email)
        {
            try
            {
                var hasHistory = await _screeningService.HasPreviousReviewHistoryAsync(email);
                return Ok(new
                {
                    IsSuccess = true,
                    Data = new { HasPreviousHistory = hasHistory }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpGet("job/{jobId}")]
        [Authorize(Roles = "Recruiter,HR,Admin,Reviewer")]
        public async Task<IActionResult> GetReviewsByJob([FromRoute] int jobId)
        {
            try
            {
                var data = await _screeningService.GetReviewsByJobAsync(jobId);
                return Ok(new { IsSuccess = true, Data = data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }


        [HttpGet("{reviewId}")]
        [Authorize(Roles = "Recruiter,HR,Admin,Reviewer,Interviewer")]
        public async Task<IActionResult> GetReviewDetails([FromRoute] int reviewId)
        {
            try
            {
                var data = await _screeningService.GetReviewDetailsAsync(reviewId);
                return Ok(new { IsSuccess = true, Data = data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }


        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }
    }
}
