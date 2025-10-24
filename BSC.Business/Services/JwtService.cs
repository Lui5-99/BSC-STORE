using System;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using BSC.Models.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using BSC.Data;
using Microsoft.EntityFrameworkCore;


namespace BSC.Business.Services
{
    public class JwtService(IConfiguration configuration, AppDbContext context)
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly AppDbContext _context = context;

        public string GenerateToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim("roleId", user.RoleId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:DurationInMinutes"]!)),
                signingCredentials: creds

            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public async Task<string> GenerateRefreshTokenAsync(User user)
        {
            var refreshToken = new RefreshToken
            {
                UserId = user.UserId,
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpirationDays"]!))
            };

            _context.RefreshToken.Add(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken.Token;
        }

        public async Task<User?> ValidateRefreshTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshToken
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == token && !r.Revoked);

            if (refreshToken == null || refreshToken.Expires < DateTime.UtcNow)
            {
                return null;
            }

            return refreshToken.User;
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshToken
                .FirstOrDefaultAsync(r => r.Token == token);
            if (refreshToken != null)
            {
                refreshToken.Revoked = true;
                await _context.SaveChangesAsync();

            }
        }
    }
}