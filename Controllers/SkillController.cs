using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recruitment_System.Data;

namespace Recruitment_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SkillController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public SkillController(ApplicationDbContext db)
        {
            _db = db;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllSkills()
        {
            try
            {
                var skills = await _db.Skills
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.SkillName)
                    .Select(s => new
                    {
                        s.SkillId,
                        s.SkillName,
                        s.Category,
                        s.SkillLevel
                    })
                    .ToListAsync();

                return Ok(new
                {
                    IsSuccess = true,
                    Data = skills
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
    }
}
