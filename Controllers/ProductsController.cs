using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewProjectFromScratch.Application.DTOs;
using NewProjectFromScratch.Application.Services;

namespace NewProjectFromScratch.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Authorize(Policy = "ProductManager")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductsController(ProductService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            try
            {
                var product = await _service.CreateProductAsync(request);
                return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ex.ParamName, ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] string? category, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            var products = await _service.GetActiveProductsAsync(category, minPrice, maxPrice);
            return Ok(products.Select(x => new ProductDto(x.Id, x.Name, x.Price, x.Stock, x.Category, x.IsActive)));
        }

        [HttpPatch("{id:guid}/stock")]
        public async Task<IActionResult> AdjustStock(Guid id, [FromBody] AdjustStockRequest request)
        {
            try
            {
                await _service.AdjustStockAsync(id, request.QuantityChange);
                return Ok(new { message = "Product updated." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeactivateProduct(Guid id)
        {
            try
            {
                await _service.DeactivateProductAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
        }
    }
}
