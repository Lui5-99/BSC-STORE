using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSC.Models.Entities
{
	public class Inventory
	{
		public int InventoryId { get; set; }
		public int ProductId { get; set; }
		public Product Product { get; set; } = null!;
		public int Quantity { get; set; } = 0;
		public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
	}
}
