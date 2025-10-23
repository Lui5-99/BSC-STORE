using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSC.Models.DTOs.Product
{
	public class ProductDto
	{
		public int ProductId { get; set; }
		public string SKU { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public decimal UnitPrice { get; set; }
		public int Quantity { get; set; }
	}
}
