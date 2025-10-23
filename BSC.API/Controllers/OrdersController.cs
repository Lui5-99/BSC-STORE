using BSC.Business.Interfaces;
using BSC.Models.DTOs.Order;
using BSC.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BSC.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class OrdersController(IOrderService orderService, IProductService productService) : Controller
	{
		private readonly IOrderService _orderService = orderService;
		private readonly IProductService _productService = productService;

		// GET: api/Orders
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var orders = await _orderService.GetAllAsync();
			return Ok(orders);
		}

		// GET: api/Orders/5
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var order = await _orderService.GetByIdAsync(id);
			if (order == null) return NotFound();
			return Ok(order);
		}

		// POST: api/Orders
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] OrderCreateDto dto)
		{
			try
			{
				var order = new Order 
				{
					OrderNumber = "Prueba",
					Items = [.. dto.Items.Select(i =>
					{
						return new OrderItem
						{
							ProductId = i.ProductId,
							Quantity = i.Quantity,
							UnitPrice = i.UnitPrice
						};
					})]
				};

				order.Total = order.Items.Sum(i => i.UnitPrice * i.Quantity);

				var created = await _orderService.CreateAsync(order);
				return CreatedAtAction(nameof(GetById), new { id = created.OrderId }, created);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
