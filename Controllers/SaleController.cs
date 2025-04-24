using Microsoft.AspNetCore.Mvc;
using app.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using app.Server.DTOs;

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
            try
            {
                if (saleRequest == null)
                {
                    return BadRequest("Invalid request: Sale data is missing.");
                }

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
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while creating the sale. Details: {ex.Message}");
            }
        }

        // GET: api/Sales/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SaleDto>> GetSale(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid sale ID. The ID must be a positive number.");
            }

            try
            {
                var sale = await _context.Sales
                                         .Include(s => s.Customer)
                                         .Include(s => s.Product)
                                         .Include(s => s.Store)
                                         .FirstOrDefaultAsync(s => s.Id == id);

                if (sale == null)
                {
                    return NotFound($"Sale with ID {id} was not found.");
                }

                var saleWithDetails = new SaleDto
                {
                    Id = sale.Id,
                    DateSold = sale.DateSold,
                    CustomerId = sale.CustomerId,
                    ProductId = sale.ProductId,
                    StoreId = sale.StoreId,
                    CustomerName = sale.Customer?.Name,
                    ProductName = sale.Product?.Name,
                    StoreName = sale.Store?.Name
                };

                return Ok(saleWithDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Unable to retrieve sale details. Error: {ex.Message}");
            }
        }

        // GET: api/Sales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetSales()
        {
            try
            {
                var sales = await _context.Sales
                                         .Include(s => s.Customer)
                                         .Include(s => s.Product)
                                         .Include(s => s.Store)
                                         .ToListAsync();

                if (sales == null || !sales.Any())
                {
                    return NotFound("No sales records found.");
                }

                var salesWithDetails = sales.Select(sale => new SaleDto
                {
                    Id = sale.Id,
                    DateSold = sale.DateSold,
                    CustomerId = sale.CustomerId,
                    ProductId = sale.ProductId,
                    StoreId = sale.StoreId,
                    CustomerName = sale.Customer?.Name,
                    ProductName = sale.Product?.Name,
                    StoreName = sale.Store?.Name
                }).ToList();

                return Ok(salesWithDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to retrieve sales list. Reason: {ex.Message}");
            }
        }

        // PUT: api/Sales/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSale(int id, [FromBody] SaleRequest saleRequest)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid sale ID. The ID must be a positive number.");
            }

            if (saleRequest == null)
            {
                return BadRequest("Invalid request: Updated sale data is missing.");
            }

            try
            {
                var sale = await _context.Sales.FindAsync(id);
                if (sale == null)
                {
                    return NotFound($"Sale with ID {id} could not be found for update.");
                }

                sale.DateSold = saleRequest.DateSold;
                sale.CustomerId = saleRequest.CustomerId;
                sale.ProductId = saleRequest.ProductId;
                sale.StoreId = saleRequest.StoreId;

                _context.Entry(sale).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to update sale with ID {id}. Error: {ex.Message}");
            }
        }

        // DELETE: api/Sales/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid sale ID. The ID must be a positive number.");
            }

            try
            {
                var sale = await _context.Sales.FindAsync(id);
                if (sale == null)
                {
                    return NotFound($"Sale with ID {id} not found for deletion.");
                }

                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while deleting the sale. Details: {ex.Message}");
            }
        }
        // GET: api/Sales/count
        [HttpGet("count")]
        public async Task<ActionResult<object>> GetTotalSales()
        {
            try
            {
                var totalSales = await _context.Sales.CountAsync();
                return Ok(new { totalSales });
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while counting total sales.");
            }
        }

    }
}
