using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recruitment_System.Data;

namespace Recruitment_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,HR,Recruiter")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _db.Users
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .OrderBy(u => u.FullName)
                    .Select(u => new
                    {
                        u.UserId,
                        u.FullName,
                        u.Email,
                        Status = u.Status, // "Active" / "Inactive"

                        Roles = u.UserRoles
                            .Select(ur => ur.Role.RoleName)
                            .ToList()
                    })
                    .ToListAsync();

                return Ok(new
                {
                    IsSuccess = true,
                    Data = users
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
