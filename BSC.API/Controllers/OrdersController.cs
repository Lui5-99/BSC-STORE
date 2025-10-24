using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BSC.Business.Interfaces;
using BSC.Models.DTOs.Order;
using BSC.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BSC.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(IOrderService orderService) : Controller
    {
        private readonly IOrderService _orderService = orderService;

        // GET: api/Orders
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue("roleName");

            bool isAdminOrSupervisor = role is "Administrador" or "Supervisor";

            IEnumerable<Order> orders = [];
            int totalCount = 0;

            if (isAdminOrSupervisor)
            {
                (orders, totalCount) = await _orderService.GetAllAsync(pageNumber, pageSize);
            }
            else
            {
                // Si es vendedor, solo ve las órdenes propias
                (orders, totalCount) = await _orderService.GetBySellerAsync(
                    int.Parse(userId),
                    pageNumber,
                    pageSize
                );
            }
            var orderDtos = orders.Select(order => new OrderDto
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                Total = order.Total,
                OrderDate = order.CreatedAt,
                SellerName = order.Seller.Username,
                Items =
                [
                    .. order.Items.Select(i => new OrderItemDto
                    {
                        OrderItemId = i.OrderItemId,
                        ProductId = i.ProductId,
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                    }),
                ],
            });

            var response = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Items = orderDtos,
            };

            return Ok(response);
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
                return NotFound();
            var orderDto = new OrderDto
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                SellerName = order.Seller.Username,
                Total = order.Total,
                Items =
                [
                    .. order.Items.Select(i => new OrderItemDto
                    {
                        ProductId = i.ProductId,
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                    }),
                ],
            };
            return Ok(orderDto);
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateDto dto)
        {
            try
            {
                var OrderNumber = $"Order-{DateTime.Now:yyyyMMddHHmmssfff}";
                var order = new Order
                {
                    OrderNumber = OrderNumber,
                    SellerUserId = dto.SellerId,
                    Items =
                    [
                        .. dto.Items.Select(i =>
                        {
                            return new OrderItem
                            {
                                ProductId = i.ProductId,
                                Quantity = i.Quantity,
                                UnitPrice = i.UnitPrice,
                            };
                        }),
                    ],
                };

                order.Total = order.Items.Sum(i => i.UnitPrice * i.Quantity);

                var created = await _orderService.CreateAsync(order);
                var createdDto = new OrderDto
                {
                    OrderId = order.OrderId,
                    OrderNumber = order.OrderNumber,
                    Total = order.Total,
                    OrderDate = order.CreatedAt,
                    SellerName = order.Seller.Username,
                    Items =
                    [
                        .. order.Items.Select(i => new OrderItemDto
                        {
                            OrderItemId = i.OrderItemId,
                            ProductId = i.ProductId,
                            ProductName = i.Product.Name,
                            Quantity = i.Quantity,
                            UnitPrice = i.UnitPrice,
                        }),
                    ],
                };
                return CreatedAtAction(nameof(GetById), new { id = created.OrderId }, createdDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
