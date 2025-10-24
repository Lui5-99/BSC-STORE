using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSC.Models.Entities
{
	public class Order
	{
		public int OrderId { get; set; }
		[Required, StringLength(50)]
		public string OrderNumber { get; set; } = null!;
		public string Customer { get; set; } = string.Empty;
		public int SellerUserId { get; set; }
		public User Seller { get; set; } = null!;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		[Column(TypeName = "decimal(18,2)")]
		public decimal Total { get; set; } = 0m;
		public ICollection<OrderItem> Items { get; set; } = [];
	}
}
