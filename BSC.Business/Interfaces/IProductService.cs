using BSC.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSC.Business.Interfaces
{
	public interface IProductService
	{
		Task<(IEnumerable<Product> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
		Task<Product?> GetByIdAsync(int id);
		Task<Product> CreateAsync(Product product, int quantity);
		Task<Product> UpdateAsync(Product product, int? quantity);
		Task<bool> DeleteAsync(int id);
	}
}
