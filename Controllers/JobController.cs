using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment_System.Dto_s.JobDtos;
using Recruitment_System.Services;
using System.Security.Claims;

namespace Recruitment_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JobController : ControllerBase
    {

        private readonly JobService _jobService;

        public JobController(JobService jobService)
        {
            _jobService = jobService;
        }

        [HttpPost]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Invalid request data", Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            try
            {
                var result = await _jobService.CreateJobAsync(request, GetCurrentUserId());
                return CreatedAtAction(nameof(GetJob), new { id = result.JobId }, new { IsSuccess = true, Message = "Job created successfully", Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }

        }

        [HttpGet]
        public async Task<IActionResult> getJobs()
        {
            try
            {
                var jobs = await _jobService.GetJobAsync();
                return Ok(new { IsSuccess = true, Message = "Jobs retrieved successfully", Data = jobs });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJob(int jobId)
        {
            try
            {
                var job = await _jobService.GetJobByIdAsync(id);
                if (job == null)
                    return NotFound(new { IsSuccess = false, Message = "Job not found" });

                return Ok(new { IsSuccess = true, Message = "Job retrieved successfully", Data = job });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Recruiter,Admin,HR")]
        public async Task<IActionResult> UpdateJob([FromBody]UpdateJobRequest request, int jobId)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Invalid request data", Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            try
            {
                var result = await _jobService.UpdateJobAsync(id, request, GetCurrentUserId());
                if (result == null)
                    return NotFound(new { IsSuccess = false, Message = "Job not found" });

                return Ok(new { IsSuccess = true, Message = "Job updated successfully", Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> DeleteJob(int jobId)
        {
            try
            {
                var result = await _jobService.DeleteJobAsync(id);
                if (!result)
                    return NotFound(new { IsSuccess = false, Message = "Job not found" });

                return Ok(new { IsSuccess = true, Message = "Job deleted successfully" });
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
