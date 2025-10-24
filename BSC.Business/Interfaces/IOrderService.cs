using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSC.Models.Entities;

namespace BSC.Business.Interfaces
{
    public interface IOrderService
    {
        Task<(IEnumerable<Order> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
        Task<(IEnumerable<Order> Items, int TotalCount)> GetBySellerAsync(
            int sellerUserId,
            int pageNumber,
            int pageSize
        );
        Task<Order?> GetByIdAsync(int id);
        Task<Order> CreateAsync(Order order);
    }
}
