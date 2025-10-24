using BSC.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSC.Business.Interfaces
{
	public interface IUserService
	{
		Task<User?> AuthenticateAsync(string username, string password);
		Task<(IEnumerable<User> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
		Task<User?> GetByIdAsync(int id);
		Task<User> CreateAsync(User user, string password);
		Task<bool> DeleteAsync(int id);
	}
}
