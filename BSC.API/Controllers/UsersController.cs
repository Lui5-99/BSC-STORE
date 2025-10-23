using BSC.Business.Interfaces;
using BSC.Models.Entities;
using BSC.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using BSC.Models.DTOs.User;

namespace BSC.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UsersController(IUserService userService) : Controller
	{
		private readonly IUserService _userService = userService;

		// GET: api/users
		[HttpGet]
		public async Task<IActionResult> GetAllUsers()
		{
			var users = await _userService.GetAllAsync();
			// Mapear a DTO para evitar ciclos y exponer solo lo necesario
			var userDtos = users.Select(u => new UserDto
			{
				Id = u.UserId,
				Username = u.Username,
				RoleId = u.Role.RoleId,
				RoleName = u.Role.Name
			}).ToList();
			return Ok(users);
		}

		// GET: api/users/{id}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetUserById(int id)
		{
			var user = await _userService.GetByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}

			var userDto = new UserDto
			{
				Id = user.UserId,
				Username = user.Username,
				RoleId = user.RoleId,
				RoleName = user.Role.Name
			};

			return Ok(userDto);
		}

		// POST: api/users
		[HttpPost]
		public async Task<IActionResult> CreateUser([FromBody] UserCreateDto dto)
		{
			var user = new User
			{
				Username = dto.Username,
				RoleId = dto.RoleId
			};

			var createdUser = await _userService.CreateAsync(user, dto.Password);

			return CreatedAtAction(nameof(GetUserById), new { id = createdUser.UserId }, createdUser);
		}

		// POST: api/Users/login
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
		{
			var user = await _userService.AuthenticateAsync(dto.Username, dto.Password);
			if (user == null) return Unauthorized("Usuario o contraseña incorrectos");

			var userDto = new UserDto
			{
				Id = user.UserId,
				Username = user.Username,
				RoleId = user.RoleId,
				RoleName = user.Role.Name
			};

			return Ok(userDto);
		}

		// DELETE: api/Users/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var result = await _userService.DeleteAsync(id);
			if (!result) return NotFound();
			return NoContent();
		}
	}
}
