using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSC.Models.Entities
{
	public class User
	{
		public int UserId { get; set; }

		[Required, StringLength(50)]
		public string Username { get; set; } = null!;

		[Required, StringLength(256)]
		public string PasswordHash { get; set; } = null!;

		[Required]
		public int RoleId { get; set; }
		public Role Role { get; set; } = null!;

		public bool IsActive { get; set; } = true;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public ICollection<Order> OrdersPlaced { get; set; } = [];
	}
}
