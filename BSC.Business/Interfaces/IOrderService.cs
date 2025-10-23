using BSC.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSC.Business.Interfaces
{
	public interface IOrderService
	{
		Task<IEnumerable<Order>> GetAllAsync();
		Task<Order?> GetByIdAsync(int id);
		Task<Order> CreateAsync(Order order);
	}
}
