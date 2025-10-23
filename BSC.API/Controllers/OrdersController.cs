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
			var orderDtos = orders.Select(order => new OrderDto
			{
				OrderId = order.OrderId,
				OrderNumber = order.OrderNumber,
				Total = order.Total,
				OrderDate = order.CreatedAt,
				SellerName = order.Seller.Username,
				Items = [.. order.Items.Select(i => new OrderItemDto
				{
					OrderItemId = i.OrderItemId,
					ProductId = i.ProductId,
					ProductName = i.Product.Name,
					Quantity = i.Quantity,
					UnitPrice = i.UnitPrice
				})]
      });
			return Ok(orderDtos);
		}

		// GET: api/Orders/5
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var order = await _orderService.GetByIdAsync(id);
			if (order == null) return NotFound();
			var orderDto = new OrderDto
			{
				OrderId = order.OrderId,
				OrderNumber = order.OrderNumber,
				SellerName = order.Seller.Username,
				Total = order.Total,
				Items = [.. order.Items.Select(i => new OrderItemDto
				{
					ProductId = i.ProductId,
					ProductName = i.Product.Name,
					Quantity = i.Quantity,
					UnitPrice = i.UnitPrice
				})]
      };
			return Ok(orderDto);
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
					SellerUserId = dto.SellerId,
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
