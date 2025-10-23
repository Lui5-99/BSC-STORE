using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BSC.Models.Entities
{
	public class Product
	{
		public int ProductId { get; set; }
		[Required, StringLength(50)]
		public string SKU { get; set; } = null!;
		[Required, StringLength(250)]
		public string Name { get; set; } = null!;
		[Column(TypeName = "decimal(18,2)")]
		public decimal UnitPrice { get; set; } = 0m;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public Inventory? Inventory { get; set; }
		[JsonIgnore]
		public ICollection<OrderItem> OrderItems { get; set; } = [];
	}
}
