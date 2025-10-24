using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSC.Business.Interfaces;
using BSC.Data;
using BSC.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BSC.Business.Services
{
    public class OrderService(AppDbContext context) : IOrderService
    {
        private readonly AppDbContext _context = context;

        public async Task<(IEnumerable<Order> Items, int TotalCount)> GetAllAsync(
            int pageNumber,
            int pageSize,
            string search
		)
        {
            // Validaciones básicas
            if (pageNumber < 1)
                pageNumber = 1;
            if (pageSize < 1)
                pageSize = 10;

            var query = _context.Order
                .Include(oi => oi.Items)
                .ThenInclude(p => p.Product)
                .Include(s => s.Seller)
                .AsQueryable();

            if(!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(o => 
                   o.OrderNumber.ToLower().Contains(search) ||
                   o.Seller.Username.ToLower().Contains(search)
                );
			}

            // Contamos el total de productos antes de paginar
            var totalCount = await query.CountAsync();

            // Aplicamos paginación directamente en la base de datos
            var items = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(IEnumerable<Order> Items, int TotalCount)> GetBySellerAsync(
            int userId,
            int pageNumber,
            int pageSize
        )
        {
            // Validaciones básicas
            if (pageNumber < 1)
                pageNumber = 1;
            if (pageSize < 1)
                pageSize = 10;

            var query = _context
                .Order.Where(o => o.SellerUserId == userId)
                .Include(oi => oi.Items)
                .ThenInclude(p => p.Product)
                .Include(s => s.Seller)
                .AsQueryable();

            // Contamos el total de productos antes de paginar
            var totalCount = await query.CountAsync();

            // Aplicamos paginación directamente en la base de datos
            var items = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Order?> GetByIdAsync(int id) =>
            await _context
                .Order.Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.Seller)
                .FirstOrDefaultAsync(o => o.OrderId == id);

        public async Task<Order> CreateAsync(Order order)
        {
            // Validar stock antes de crear
            foreach (var item in order.Items)
            {
                var inventory = await _context.Inventory.Include(p => p.Product).FirstOrDefaultAsync(i =>
                    i.ProductId == item.ProductId
                );

                var product = await _context.Product.FirstOrDefaultAsync(p => p.ProductId == item.ProductId);

                if (inventory == null || inventory.Quantity < item.Quantity)
                    throw new InvalidOperationException(
                        $"No hay suficiente stock del producto {product?.Name}"
                    );

                inventory.Quantity -= item.Quantity;
            }

            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            await _context.Entry(order).Reference(o => o.Seller).LoadAsync();
            await _context.Entry(order).Collection(o => o.Items).LoadAsync();
            foreach (var item in order.Items)
            {
                await _context.Entry(item).Reference(i => i.Product).LoadAsync();
            }

            return order;
        }
    }
}
