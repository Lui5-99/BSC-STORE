using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSC.Models.DTOs.Order
{
	public class OrderItemDto
	{
		public int ProductId { get;  set; }
		public string ProductName { get; set; } = string.Empty;
		public int Quantity { get; set; }
		public decimal UnitPrice { get; set; }
	}
}
