
using BackendAPI.Interfaces;
using BackendAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackendAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var secretKey =
                jwtSettings["Key"]
                ?? throw new Exception("JwtSettings:Key chưa được cấu hình.");

            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey));

            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                // ID người dùng
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                // Dùng cho User.Identity.Name
                new Claim(
                    ClaimTypes.Name,
                    user.Email ?? string.Empty),

                // Email
                new Claim(
                    ClaimTypes.Email,
                    user.Email ?? string.Empty),

                // Quyền
                new Claim(
                    ClaimTypes.Role,
                    string.IsNullOrEmpty(user.Role)
                        ? "User"
                        : user.Role),

                // Streak
                new Claim(
                    "CurrentStreak",
                    user.CurrentStreak.ToString()),

                // EXP
                new Claim(
                    "TotalExp",
                    user.TotalExp.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),

                Expires = DateTime.UtcNow.AddDays(7),

                Issuer = jwtSettings["Issuer"],

                Audience = jwtSettings["Audience"],

                SigningCredentials = credentials
            };

            var tokenHandler =
                new JwtSecurityTokenHandler();

            var securityToken =
                tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }
    }
}