using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment_System.Dto_s.InterviewDtos;
using Recruitment_System.Services;
using System.Security.Claims;

namespace Recruitment_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InterviewController : ControllerBase
    {
        private readonly InterviewService _interviewService;

        public InterviewController(InterviewService interviewService)
        {
            _interviewService = interviewService;
        }

       
        [HttpPost("start/{reviewId}")]
        [Authorize(Roles = "Recruiter,Admin,HR")]
        public async Task<IActionResult> StartProcess(int reviewId, [FromBody] StartInterviewProcessRequest request)
        {
            try
            {
                var result = await _interviewService.StartInterviewProcessAsync(reviewId, request.TotalRounds);
                return Ok(new { IsSuccess = true, Data = new { result.InterviewProcessId } });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        
        [HttpPost("{processId}/rounds")]
        [Authorize(Roles = "Recruiter,Admin,HR")]
        public async Task<IActionResult> CreateRound(int processId, [FromBody] CreateRoundRequest request)
        {
            try
            {
                var round = await _interviewService.CreateRoundAsync(processId, request.RoundNumber, request.RoundType);
                return Ok(new { IsSuccess = true, Data = new { round.InterviewRoundId } });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        
        [HttpPut("rounds/{roundId}/schedule")]
        [Authorize(Roles = "Recruiter,Admin,HR")]
        public async Task<IActionResult> ScheduleRound(int roundId, [FromBody] ScheduleRoundRequest request)
        {
            try
            {
                await _interviewService.ScheduleRoundAsync(roundId, request.ScheduledAt, request.Mode, request.MeetingLinkOrLocation);
                return Ok(new { IsSuccess = true, Message = "Round scheduled" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpPost("rounds/{roundId}/panel")]
        [Authorize(Roles = "Recruiter,Admin,HR")]
        public async Task<IActionResult> AddPanelMember(int roundId, [FromBody] AddPanelMemberRequest request)
        {
            try
            {
                await _interviewService.AddPanelMemberAsync(roundId, request.InterviewerUserId);
                return Ok(new { IsSuccess = true, Message = "Panel member added" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

       
        [HttpPost("rounds/{roundId}/feedback")]
        [Authorize(Roles = "Interviewer")]
        public async Task<IActionResult> SubmitFeedback(int roundId, [FromBody] SubmitFeedbackRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _interviewService.SubmitFeedbackAsync(roundId, userId, request.Rating, request.Recommendation, request.Comments);
                return Ok(new { IsSuccess = true, Message = "Feedback submitted" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        
        [HttpPut("rounds/{roundId}/complete")]
        [Authorize(Roles = "Recruiter,Admin,HR")]
        public async Task<IActionResult> CompleteRound(int roundId)
        {
            try
            {
                await _interviewService.CompleteRoundAsync(roundId);
                return Ok(new { IsSuccess = true, Message = "Round completed" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpGet("process/{processId}")]
        [Authorize(Roles = "Recruiter,Admin,HR,Interviewer")]
        public async Task<IActionResult> GetProcessDetails(int processId)
        {
            try
            {
                var data = await _interviewService.GetInterviewProcessDetailsAsync(processId);
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
