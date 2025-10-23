using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSC.Models.DTOs.Product
{
	public class CreateProductDto
	{
		[Required]
		[StringLength(50)]
		public string SKU { get; set; } = string.Empty;

		[Required]
		[StringLength(200)]
		public string Name { get; set; } = string.Empty;

		[Range(0.01, double.MaxValue)]
		public decimal UnitPrice { get; set; }

		[Range(0, int.MaxValue)]
		public int InitialQuantity { get; set; }
	}
}
