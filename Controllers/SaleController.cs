using Microsoft.AspNetCore.Mvc;
using app.Server.Models;  // Ensure this is referencing the correct namespace for SaleRequest
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace app.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly StoreDbContext _context;

        public SalesController(StoreDbContext context)
        {
            _context = context;
        }

        // POST: api/Sales
        [HttpPost(Name = "AddSale")]
        public async Task<ActionResult<Sale>> AddSale([FromBody] SaleRequest saleRequest)
        {
            var sale = new Sale
            {
                DateSold = saleRequest.DateSold,
                CustomerId = saleRequest.CustomerId,
                ProductId = saleRequest.ProductId,
                StoreId = saleRequest.StoreId
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSale), new { id = sale.Id }, sale);
        }

        // GET: api/Sales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sale>> GetSale(int id)
        {
            var sale = await _context.Sales
                                     .Include(s => s.Customer)
                                     .Include(s => s.Product)
                                     .Include(s => s.Store)
                                     .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
            {
                return NotFound();
            }

            var saleWithDetails = new
            {
                sale.Id,
                sale.DateSold,
                sale.CustomerId,
                sale.ProductId,
                sale.StoreId,
                CustomerName = sale.Customer?.Name,
                ProductName = sale.Product?.Name,
                StoreName = sale.Store?.Name
            };

            return Ok(saleWithDetails);
        }

        // GET: api/Sales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetSales()
        {
            var sales = await _context.Sales
                                     .Include(s => s.Customer)
                                     .Include(s => s.Product)
                                     .Include(s => s.Store)
                                     .ToListAsync();

            var salesWithDetails = sales.Select(sale => new
            {
                sale.Id,
                sale.DateSold,
                sale.CustomerId,
                sale.ProductId,
                sale.StoreId,
                CustomerName = sale.Customer?.Name,
                ProductName = sale.Product?.Name,
                StoreName = sale.Store?.Name
            }).ToList();

            return Ok(salesWithDetails);
        }

        // PUT: api/Sales/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSale(int id, [FromBody] SaleRequest saleRequest)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            sale.DateSold = saleRequest.DateSold;
            sale.CustomerId = saleRequest.CustomerId;
            sale.ProductId = saleRequest.ProductId;
            sale.StoreId = saleRequest.StoreId;

            _context.Entry(sale).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Sales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
