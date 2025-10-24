using BSC.Business.Services;
using BSC.Data;
using BSC.Models.DTOs.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BSC.API.utils;

namespace BSC.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController(AppDbContext context, JwtService jwt) : Controller
	{
		private readonly AppDbContext _context = context;
		private readonly JwtService _jwtService = jwt;

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
		{
			var user = await _context.User.FirstOrDefaultAsync(u => u.Username == dto.Username);
			if(user == null || user.PasswordHash != Utils.ComputeHash(dto.Password))
			{
				return Unauthorized(new { message = "Nombre de usuario o password invalido" });
			}

			var token = _jwtService.GenerateToken(user);
			var refreshToken = await _jwtService.GenerateRefreshTokenAsync(user);

			return Ok(new { token, refreshToken });
		}

		[HttpPost("refresh")]
		public async Task<IActionResult> Refresh([FromBody] string refreshToken)
		{
			var user = await _jwtService.ValidateRefreshTokenAsync(refreshToken);
			if (user == null)
				return Unauthorized("Token inválido o expirado");

			var accessToken = _jwtService.GenerateToken(user);
			var newRefreshToken = await _jwtService.GenerateRefreshTokenAsync(user);

			await _jwtService.RevokeRefreshTokenAsync(refreshToken);

			return Ok(new { accessToken, refreshToken = newRefreshToken });
		}

		[HttpPost("logout")]
		public async Task<IActionResult> Logout([FromBody] string refreshToken)
		{
			await _jwtService.RevokeRefreshTokenAsync(refreshToken);
			return Ok("Sesión cerrada correctamente");
		}
	}
}
