using BSC.API.utils;
using BSC.Business.Interfaces;
using BSC.Business.Services;
using BSC.Models.DTOs;
using BSC.Models.DTOs.User;
using BSC.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BSC.API.Controllers
{
    [Authorize(Roles = "Administrador")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserService userService) : Controller
    {
        private readonly IUserService _userService = userService;

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(int pageNumber = 1, int pageSize = 10, string search = "")
        {
            var (users, totalCount) = await _userService.GetAllAsync(pageNumber, pageSize, search);
            // Mapear a DTO para evitar ciclos y exponer solo lo necesario
            var userDtos = users
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    RoleId = u.Role.RoleId,
                    RoleName = u.Role.Name,
                })
                .ToList();

            var response = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Items = userDtos,
            };
            return Ok(response);
        }

        // GET: api/users/get-search
        [HttpGet("get-search")]
        public async Task<IActionResult> GetAllUsersSearch()
        {
            var users = await _userService.GetAllToSearch();
            var usersDto = users
                .Select(u => new UserDto { UserId = u.UserId, Username = u.Username })
                .ToList();

            return Ok(usersDto);
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
                UserId = user.UserId,
                Username = user.Username,
                RoleId = user.RoleId,
                RoleName = user.Role.Name,
            };

            return Ok(userDto);
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto dto)
        {
            var user = new User { Username = dto.Username, RoleId = dto.RoleId };

            var createdUser = await _userService.CreateAsync(user, dto.Password);

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = createdUser.UserId },
                createdUser
            );
        }

        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var user = await _userService.AuthenticateAsync(dto.Username, dto.Password);
            if (user == null)
                return Unauthorized("Usuario o contraseña incorrectos");

            var userDto = new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                RoleId = user.RoleId,
                RoleName = user.Role.Name,
            };

            return Ok(userDto);
        }

        // UPDATE: api/Users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            var user = new User
            {
                UserId = id,
                Username = dto.Username,
                PasswordHash = Utils.ComputeHash(dto.Password),
                RoleId = dto.RoleId,
            };
            await _userService.UpdateAsync(user);

            return Ok(user);
        }

        // DELETE: api/Users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
