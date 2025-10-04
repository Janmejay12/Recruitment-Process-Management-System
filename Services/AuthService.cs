using Microsoft.EntityFrameworkCore;
using Recruitment_System.Data;

namespace Recruitment_System.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly JwtService _jwt;

        public AuthService(ApplicationDbContext db, JwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        public async Task<(bool ok, string message, string token, string fullName, string email, string[] roles)> LoginAsync(string email, string password, bool rememberMe)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return (false, "Email and password are required", "", "", "", []);

            var user = await _db.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null) return (false, "Invalid email or password", "", "", "", []);
            if (user.Status != "Active") return (false, "Account is inactive", "", "", "", []);
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return (false, "Invalid email or password", "", "", "", []);

            var roles = user.UserRoles?.Select(r => r.Role.RoleName).ToArray() ?? [];
            var expiryHours = rememberMe ? 168 : 24;
            var token = _jwt.GenerateToken(user.UserId, user.FullName, user.Email, roles, expiryHours);

            return (true, "Login successful", token, user.FullName, user.Email, roles);
        }
    }
}
