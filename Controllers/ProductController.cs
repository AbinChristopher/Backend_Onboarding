using Microsoft.AspNetCore.Mvc;
using app.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using app.Server.DTOs;

namespace app.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly StoreDbContext _context;

        public ProductsController(StoreDbContext context)
        {
            _context = context;
        }

        // POST: api/Products
        [HttpPost(Name = "AddProduct")]
        public async Task<ActionResult<ProductDTO>> AddProduct([FromBody] ProductRequest productRequest)
        {
            if (productRequest == null || string.IsNullOrWhiteSpace(productRequest.Name) || productRequest.Price == null)
            {
                return BadRequest("Invalid product data.");
            }

            try
            {
                var product = new Product
                {
                    Name = productRequest.Name,
                    Price = productRequest.Price.Value
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                var productDTO = new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price
                };

                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDTO);
            }
            catch
            {
                return StatusCode(500, "An error occurred while creating the product.");
            }
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID.");
            }

            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                var productDTO = new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price
                };

                return productDTO;
            }
            catch
            {
                return StatusCode(500, "An error occurred while retrieving the product.");
            }
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            try
            {
                var products = await _context.Products.ToListAsync();

                var productDTOs = products.Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price
                }).ToList();

                return productDTOs;
            }
            catch
            {
                return StatusCode(500, "An error occurred while retrieving the product list.");
            }
        }

        //GET: api/Products/count
        [HttpGet("count")]
        public async Task<ActionResult<object>> GetProductCount()
        {
            try
            {
                var count = await _context.Products.CountAsync();
                return Ok(new { count });
            }
            catch
            {
                return StatusCode(500, "An error occurred while counting products.");
            }
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductRequest productRequest)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID.");
            }

            if (productRequest == null || string.IsNullOrWhiteSpace(productRequest.Name) || productRequest.Price == null)
            {
                return BadRequest("Invalid product data.");
            }

            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                product.Name = productRequest.Name;
                product.Price = productRequest.Price.Value;

                _context.Entry(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(500, "An error occurred while updating the product.");
            }
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID.");
            }

            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return StatusCode(500, "An error occurred while deleting the product.");
            }
        }
    }

    public class ProductRequest
    {
        public string Name { get; set; }
        public decimal? Price { get; set; }
    }
}
