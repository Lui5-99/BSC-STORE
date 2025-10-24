using BSC.Business.Interfaces;
using BSC.Data;
using BSC.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BSC.Business.Services
{
	public class UserService(AppDbContext context) : IUserService
	{
		private readonly AppDbContext _context = context;

		public async Task<User?> AuthenticateAsync(string username, string password)
		{
			var user = await _context.User.Include(u => u.Role)
				.FirstOrDefaultAsync(u => u.Username == username);

			if (user == null || user.PasswordHash != ComputeHash(password))
				return null;

			return user;
		}

		public async Task<(IEnumerable<User> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize)
		{
			// Validaciones básicas
			if (pageNumber < 1) pageNumber = 1;
			if (pageSize < 1) pageSize = 10;

			var query = _context.User
				.Include(p => p.Role)
				.AsQueryable();

			// Contamos el total de productos antes de paginar
			var totalCount = await query.CountAsync();

			// Aplicamos paginación directamente en la base de datos
			var items = await query
				.OrderBy(u => u.Username)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (items, totalCount);
		}

		public async Task<User?> GetByIdAsync(int id) =>
			await _context.User.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id);

		public async Task<User> CreateAsync(User user, string password)
		{
			user.PasswordHash = ComputeHash(password);
			_context.User.Add(user);
			await _context.SaveChangesAsync();
			return user;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var user = await _context.User.FindAsync(id);
			if (user == null) return false;

			_context.User.Remove(user);
			await _context.SaveChangesAsync();
			return true;
		}

		private static string ComputeHash(string input)
		{
			var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
			return Convert.ToBase64String(bytes);
		}
	}
}
