using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSC.Models.Entities
{
	public class RefreshToken
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public string Token { get; set; } = string.Empty;
		public DateTime Expires { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public bool Revoked { get; set; } = false;

		public User? User { get; set; }
	}
}
