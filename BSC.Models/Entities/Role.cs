using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BSC.Models.Entities
{
	public class Role
	{
		public int RoleId { get; set; }
		public string Name { get; set; } = null!;
		[JsonIgnore]
		public ICollection<User> Users { get; set; } = [];
	}
}
