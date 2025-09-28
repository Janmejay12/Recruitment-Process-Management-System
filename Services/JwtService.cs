using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Recruitment_System.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;
        private readonly JwtSecurityTokenHandler _handler = new();

        public JwtService(IConfiguration config)
        {
            _config = config;
        }
        public string GenerateToken(int userId, string fullName, string email, IEnumerable<string> roles, int expiryHours)
        {
            var jwt = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Name, fullName),
                new(ClaimTypes.Email, email)
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expiryHours),
                signingCredentials: creds
            );

            return _handler.WriteToken(token);
        }
    }
}
