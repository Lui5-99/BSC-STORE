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
	public class ProductService(AppDbContext context) : IProductService
	{
		private readonly AppDbContext _context = context;

		public async Task<(IEnumerable<Product> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize)
		{
			// Validaciones básicas
			if (pageNumber < 1) pageNumber = 1;
			if (pageSize < 1) pageSize = 10;

			var query = _context.Product
				.Include(p => p.Inventory)
				.AsQueryable();

			// Contamos el total de productos antes de paginar
			var totalCount = await query.CountAsync();

			// Aplicamos paginación directamente en la base de datos
			var items = await query
				.OrderBy(p => p.ProductId)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (items, totalCount);
		}

		public async Task<Product?> GetByIdAsync(int id) =>
			await _context.Product.Include(p => p.Inventory)
				.FirstOrDefaultAsync(p => p.ProductId == id);

		public async Task<Product> CreateAsync(Product product, int initialQuantity = 0)
		{
			// 1. Agregar el producto
			_context.Product.Add(product);
			await _context.SaveChangesAsync(); // Esto guarda ProductId generado

			// 2️. Crear inventario para el producto recién creado
			var inventory = new Inventory
			{
				ProductId = product.ProductId,
				Quantity = initialQuantity,
				LastUpdated = DateTime.UtcNow
			};

			_context.Inventory.Add(inventory);
			await _context.SaveChangesAsync();

			// 3️. Devolver producto con inventario cargado (opcional)
			product.Inventory = inventory;
			return product;
		}

		public async Task<Product> UpdateAsync(Product product, int? newQuantity = null)
		{
			// 1️. Actualizar el producto
			_context.Product.Update(product);

			// 2️. Si se proporciona una nueva cantidad, actualizar el inventario
			if (newQuantity.HasValue)
			{
				var inventory = await _context.Inventory
					.FirstOrDefaultAsync(i => i.ProductId == product.ProductId);

				if (inventory != null)
				{
					// Actualizar inventario existente
					inventory.Quantity = newQuantity.Value;
					inventory.LastUpdated = DateTime.UtcNow;
					_context.Inventory.Update(inventory);
				}
				else
				{
					// Crear inventario nuevo
					inventory = new Inventory
					{
						ProductId = product.ProductId,
						Quantity = newQuantity.Value,
						LastUpdated = DateTime.UtcNow
					};
					_context.Inventory.Add(inventory);
				}
			}

			// 3️. Guardar cambios
			await _context.SaveChangesAsync();

			// 4️. Devolver el producto actualizado
			if (newQuantity.HasValue)
				product.Inventory = await _context.Inventory
					.FirstOrDefaultAsync(i => i.ProductId == product.ProductId)!;

			return product;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var product = await _context.Product.FindAsync(id);
			if (product == null) return false;
			_context.Inventory.RemoveRange(_context.Inventory.Where(i => i.ProductId == id));
			_context.Product.Remove(product);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
