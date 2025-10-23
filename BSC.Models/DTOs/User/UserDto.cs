using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSC.Models.DTOs.User
{
	public class UserDto
	{
		public int Id { get; set; }
		public string Username { get; set; } = string.Empty;
		public int RoleId { get; set; }
		public string RoleName { get; set; } = string.Empty;
	}
}
