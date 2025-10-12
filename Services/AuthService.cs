using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Recruitment_System.Data;
using Recruitment_System.Dto_s;
using Recruitment_System.Entities;

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

        public async Task<LoginResponse> LoginAsync(string email, string password, bool rememberMe)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return new LoginResponse { IsSuccess = false, Message = "Email and password are required" };

            var user = await _db.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null) return new LoginResponse { IsSuccess = false, Message = "Invalid email or password" };
            if (user.Status != "Active") return new LoginResponse { IsSuccess = false, Message = "Account is inactive" };
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return new LoginResponse {IsSuccess = false, Message = "Invalid email or password"};

            var roleList = user.UserRoles != null
                ? user.UserRoles.Select(r => r.Role.RoleName).ToArray()
                : Array.Empty<string>();

            var roles = roleList;
            var expiryHours = rememberMe ? 168 : 24;
            var token = _jwt.GenerateToken(user.UserId, user.FullName, user.Email, roles, expiryHours);
           

            return new LoginResponse { IsSuccess = true, Message = "Login successful", Token = token, FullName = user.FullName, Email = user.Email, Roles = roles.ToList(), ExpiresAt = DateTime.UtcNow.AddHours(expiryHours) };
        }

        // Admin registration (for internal users)
        public async Task<RegisterResponse> AdminRegisterAsync(AdminRegisterRequest request, int adminUserId)
        {
            try {

                var adminUser = await _db.Users
                   .Include(u => u.UserRoles)
                   .ThenInclude(ur => ur.Role)
                   .FirstOrDefaultAsync(u => u.UserId == adminUserId);

                if (adminUser == null)
                    return new RegisterResponse { IsSuccess = false, Message = "Admin user not found" };

                if (adminUser.Status != "Active")
                    return new RegisterResponse { IsSuccess = false, Message = "Admin account is inactive" };

                var adminRoles = adminUser.UserRoles?.Select(r => r.Role.RoleName).ToList() ?? [];
                if(!adminRoles.Any(r => r == "Admin" || r == "HR"))
                    return new RegisterResponse { IsSuccess = false, Message = "Insufficient permissions. Only Admin or HR can register users" };

                if (await _db.Users.AnyAsync(u => u.Email == request.Email))
                {
                    return new RegisterResponse { IsSuccess = false, Message = "Email already exists" };
                }

                var validRoles = new[] { "Admin", "Recruiter", "HR", "Interviewer", "Reviewer" };
                if (!validRoles.Contains(request.Role))
                {
                    return new RegisterResponse { IsSuccess = false, Message = "Invalid role" };
                }

                var user = new User
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Status = "Active"
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                var role = await _db.Roles.FirstOrDefaultAsync(r => request.Role == r.RoleName);
                if(role == null)
                    return new RegisterResponse { IsSuccess = false, Message = "Role not found" };

                var userRole = new UserRole
                {
                    UserId = user.UserId,
                    RoleId = role.RoleId
                };

                _db.UserRoles.Add(userRole);
                await _db.SaveChangesAsync();

                return new RegisterResponse
                {
                    IsSuccess = true,
                    Message = "User registered successfully",
                    UserId = user.UserId.ToString(),
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = request.Role
                };
            }
            catch (Exception ex)
            {
                return new RegisterResponse { IsSuccess = false, Message = "Registration failed: " + ex.Message };
            }
        }


        // Candidate self-registration
        public async Task<RegisterResponse> CandidateRegisterAsync(CandidateRegisterRequest request)
        {
            try
            {
                // Check if email already exists
                if (await _db.Users.AnyAsync(u => u.Email == request.Email))
                    return new RegisterResponse { IsSuccess = false, Message = "Email already exists" };

                // Create user
                var user = new User
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Status = "Active"
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                // Get Candidate role
                var candidateRole = await _db.Roles.FirstOrDefaultAsync(r => r.RoleName == "Candidate");
                if (candidateRole == null)
                    return new RegisterResponse { IsSuccess = false, Message = "Candidate role not found" };

                // Assign Candidate role
                var userRole = new UserRole
                {
                    UserId = user.UserId,
                    RoleId = candidateRole.RoleId
                };

                _db.UserRoles.Add(userRole);
                await _db.SaveChangesAsync();

                return new RegisterResponse
                {
                    IsSuccess = true,
                    Message = "Registration successful",
                    UserId = user.UserId.ToString(),
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = "Candidate"
                };
            }
            catch (Exception ex)
            {
                return new RegisterResponse { IsSuccess = false, Message = "Registration failed: " + ex.Message };
            }
        }
    }
}
