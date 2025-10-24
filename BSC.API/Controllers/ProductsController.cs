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
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, string search = "")
        {
            var (products, totalCount) = await _productService.GetAllAsync(pageNumber, pageSize, search);

            var productsDtos = products
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    SKU = p.SKU,
                    Name = p.Name,
                    UnitPrice = p.UnitPrice,
                    Quantity = p.Inventory?.Quantity ?? 0,
                })
                .ToList();

            var response = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Items = productsDtos,
            };

            return Ok(response);
        }

        // GET: api/users/get-search
        [HttpGet("get-search")]
        public async Task<IActionResult> GetAllUsersSearch()
        {
            var products = await _productService.GetAllToSearch();
            var productsDto = products
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    SKU = p.SKU,
                    Name = p.Name,
                    Quantity = p.Inventory?.Quantity ?? 0,
                    UnitPrice = p.UnitPrice,
                })
                .ToList();

            return Ok(productsDto);
        }

        // GET: api/Products/5
        [Authorize(Roles = "Administrador,Supervisor")]
		[HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            var productDto = new ProductDto
            {
                ProductId = product.ProductId,
                SKU = product.SKU,
                Name = product.Name,
                UnitPrice = product.UnitPrice,
                Quantity = product.Inventory?.Quantity ?? 0,
            };

            return Ok(productDto);
        }

        // POST: api/Products
        [Authorize(Roles = "Administrador,Supervisor")]
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
                Quantity = created.Inventory?.Quantity ?? 0,
            };
            return CreatedAtAction(nameof(GetById), new { id = created.ProductId }, createdDto);
        }

        // PUT: api/Products/5
        [Authorize(Roles = "Administrador,Supervisor")]
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
                Quantity = updated.Inventory?.Quantity ?? 0,
            };
            return Ok(updatedDto);
        }

        // DELETE: api/Products/5
        [Authorize(Roles = "Administrador")]
		[HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpGet("export-excel")]
        [Authorize(Roles = "Administrador,Supervisor")]
        public async Task<IActionResult> ExportExcel()
        {
            try
            {
                var products = await _productService.GetAllToSearch();

                using var workbook = new ClosedXML.Excel.XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Productos");

                // Encabezados
                worksheet.Cell(1, 1).Value = "SKU";
                worksheet.Cell(1, 2).Value = "Nombre";
                worksheet.Cell(1, 3).Value = "Precio Unitario";
                worksheet.Cell(1, 4).Value = "Stock";

                // Rellenar datos
                int row = 2; // la fila 1 tiene encabezados
                foreach (var product in products)
                {
                    worksheet.Cell(row, 1).Value = product.SKU;
                    worksheet.Cell(row, 2).Value = product.Name;
                    worksheet.Cell(row, 3).Value = product.UnitPrice;
                    worksheet.Cell(row, 4).Value = product.Inventory?.Quantity ?? 0;
                    row++;
                }

                // Formato simple
                worksheet.Columns().AdjustToContents();

                // 🔹 Convertir a stream y devolverlo como archivo
                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);

                var fileName = $"productos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(
                    stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Error al generar el archivo Excel");
            }
        }
    }
}
