using Microsoft.AspNetCore.Mvc;
using app.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace app.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly StoreDbContext _context;

        public CustomersController(StoreDbContext context)
        {
            _context = context;
        }

        // POST: api/Customers
        [HttpPost(Name = "AddCustomer")]
        public async Task<ActionResult<Customer>> AddCustomer([FromBody] CustomerRequest customerRequest)
        {
            // 1. Create a new Customer with the provided information
            var customer = new Customer
            {
                Name = customerRequest.Name,
                Address = customerRequest.Address
            };

            // 2. Add customer to the database
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync(); // Save the customer and generate the Customer ID

            // 3. Return the newly created customer
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        // PUT: api/Customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerRequest customerRequest)
        {
            // Check if the customer exists
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Update customer fields
            customer.Name = customerRequest.Name;
            customer.Address = customerRequest.Address;

            // Save the changes to the database
            _context.Entry(customer).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Return a success response
            return NoContent();
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            // Find the customer to delete
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Remove the customer from the database
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            // Return a success response
            return NoContent();
        }
    }

    // Request body for adding or updating a customer
    public class CustomerRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
