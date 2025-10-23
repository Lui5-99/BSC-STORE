using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSC.Models.DTOs.Order
{
	public class OrderDto
	{
		public int OrderId { get; set; }
		public string OrderNumber { get; set; } = string.Empty;
		public decimal Total { get; set; }	
		public DateTime OrderDate { get; set; }
		public string SellerName { get; set; } = string.Empty;
		public List<OrderItemDto> Items { get; set; } = [];
	}
}
