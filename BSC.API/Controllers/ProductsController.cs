using BSC.Business.Interfaces;
using BSC.Models.DTOs.Product;
using BSC.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BSC.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProductsController(IProductService productService) : Controller
	{
		private readonly IProductService _productService = productService;

		// GET: api/Products
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var products = await _productService.GetAllAsync();
			var productsDtos = products.Select(p => new ProductDto
			{
				ProductId = p.ProductId,
				SKU = p.SKU,
				Name = p.Name,
				UnitPrice = p.UnitPrice,
				Quantity = p.Inventory?.Quantity ?? 0
			}).ToList();
			return Ok(productsDtos);
		}

		// GET: api/Products/5
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var product = await _productService.GetByIdAsync(id);
			if (product == null) return NotFound();

			var productDto = new ProductDto
			{
				ProductId = product.ProductId,
				SKU = product.SKU,
				Name = product.Name,
				UnitPrice = product.UnitPrice,
				Quantity = product.Inventory?.Quantity ?? 0
			};

			return Ok(productDto);
		}

		// POST: api/Products
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] ProductDto dto)
		{
			var product = new Product
			{
				SKU = dto.SKU,
				Name = dto.Name,
				UnitPrice = dto.UnitPrice,
			};
			var created = await _productService.CreateAsync(product, dto.Quantity);
			return CreatedAtAction(nameof(GetById), new { id = created.ProductId }, created);
		}

		// PUT: api/Products/5
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] ProductDto dto)
		{
			var product = new Product
			{
				ProductId = id,
				SKU = dto.SKU,
				Name = dto.Name,
				UnitPrice = dto.UnitPrice,
			};
			var updated = await _productService.UpdateAsync(product, dto.Quantity);
			var updatedDto = new ProductDto
			{
				ProductId = updated.ProductId,
				SKU = updated.SKU,
				Name = updated.Name,
				UnitPrice = updated.UnitPrice,
				Quantity = updated.Inventory?.Quantity ?? 0
			};
			return Ok(updatedDto);
		}

		// DELETE: api/Products/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var result = await _productService.DeleteAsync(id);
			if (!result) return NotFound();
			return NoContent();
		}
	}
}
