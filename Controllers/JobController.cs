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
        public async Task<IActionResult> GetJobs()
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
        public async Task<IActionResult> GetJob([FromRoute] int id)
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
        public async Task<IActionResult> UpdateJob([FromRoute] int id, [FromBody] UpdateJobRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Invalid request data", Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            try
            {
                var result = await _jobService.UpdateJobAsync(id, request);
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
        public async Task<IActionResult> DeleteJob([FromRoute] int id)
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

        [HttpPut("{id}/hold")]
        [Authorize(Roles = "Recruiter,Admin,HR")]
        public async Task<IActionResult> PutOnHold([FromRoute] int id, [FromBody] OnHoldOrOnCloseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Invalid request", Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            try
            {
                var result = await _jobService.PutJobOnHoldAsync(id, request.Reason);
                return Ok(new { IsSuccess = true, Message = "Job put on hold", Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        
        [HttpPut("{id}/resume")]
        [Authorize(Roles = "Recruiter,Admin,HR")]
        public async Task<IActionResult> Resume([FromRoute] int id)
        {
            try
            {
                var result = await _jobService.ResumeJobAsync(id);
                return Ok(new { IsSuccess = true, Message = "Job resumed", Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

        
        [HttpPut("{id}/close")]
        [Authorize(Roles = "Recruiter,Admin,HR")]
        public async Task<IActionResult> CloseJob([FromRoute] int id, [FromBody] OnHoldOrOnCloseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Invalid request", Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            try
            {
                var result = await _jobService.CloseJobAsync(id, request.Reason, GetCurrentUserId());
                return Ok(new { IsSuccess = true, Message = "Job closed", Data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { IsSuccess = false, Message = ex.Message });
            }
        }

       
        [HttpPut("{id}/close-with-candidate")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> CloseWithCandidate([FromRoute] int id, [FromBody] CloseWithCandidateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { IsSuccess = false, Message = "Invalid request", Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            try
            {
                var result = await _jobService.CloseJobWithCandidateAsync(
                    id,
                    request.CandidateId,
                    request.SelectionDate,
                    request.Reason,
                    request.Notes,
                    GetCurrentUserId());

                return Ok(new { IsSuccess = true, Message = "Job closed with selected candidate", Data = result });
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
