using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSC.Models.DTOs.Order
{
	public class OrderCreateDto
	{
		[Required]
		public int SellerId { get; set; } 

		[Required]
		public List<OrderItemCreateDto> Items { get; set; } = [];
	}
}
