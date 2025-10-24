using BSC.Business.Interfaces;
using BSC.Data;
using BSC.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSC.Business.Services
{
	public class OrderService(AppDbContext context) : IOrderService
	{
		private readonly AppDbContext _context = context;

		public async Task<(IEnumerable<Order> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize)
		{
			// Validaciones básicas
			if (pageNumber < 1) pageNumber = 1;
			if (pageSize < 1) pageSize = 10;

			var query = _context.Order
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
			await _context.Order
				.Include(o => o.Items)
				.ThenInclude(i => i.Product)
				.Include(o => o.Seller)
				.FirstOrDefaultAsync(o => o.OrderId == id);

		public async Task<Order> CreateAsync(Order order)
		{
			// Validar stock antes de crear
			foreach (var item in order.Items)
			{
				var inventory = await _context.Inventory
					.FirstOrDefaultAsync(i => i.ProductId == item.ProductId);

				if (inventory == null || inventory.Quantity < item.Quantity)
					throw new InvalidOperationException($"No hay suficiente stock del producto {item.ProductId}");

				inventory.Quantity -= item.Quantity;
			}

			_context.Order.Add(order);
			await _context.SaveChangesAsync();
			return order;
		}
	}
}
