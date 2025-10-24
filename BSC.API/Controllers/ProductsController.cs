using BSC.Business.Interfaces;
using BSC.Models.DTOs.Product;
using BSC.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BSC.API.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class ProductsController(IProductService productService) : Controller
	{
		private readonly IProductService _productService = productService;

		// GET: api/Products
		[HttpGet]
		public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
		{
			var (products, totalCount) = await _productService.GetAllAsync(pageNumber, pageSize);

			var productsDtos = products.Select(p => new ProductDto
			{
				ProductId = p.ProductId,
				SKU = p.SKU,
				Name = p.Name,
				UnitPrice = p.UnitPrice,
				Quantity = p.Inventory?.Quantity ?? 0
			}).ToList();

			var response = new
			{
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalCount = totalCount,
				TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
				Items = productsDtos
			};

			return Ok(response);
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
			var createdDto = new ProductDto
			{
				ProductId = created.ProductId,
				SKU = created.SKU,
				Name = created.Name,
				UnitPrice = created.UnitPrice,
				Quantity = created.Inventory?.Quantity ?? 0
			};
			return CreatedAtAction(nameof(GetById), new { id = created.ProductId }, createdDto);
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
