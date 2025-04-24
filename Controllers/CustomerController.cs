using app.Server.Models;
using app.Server.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

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
        public async Task<ActionResult<CustomerDTO>> AddCustomer([FromBody] CustomerRequest customerRequest)
        {
            if (customerRequest == null || string.IsNullOrWhiteSpace(customerRequest.Name) || string.IsNullOrWhiteSpace(customerRequest.Address))
            {
                return BadRequest("Customer details are required. Please ensure Name and Address are provided.");
            }

            try
            {
                var customer = new Customer
                {
                    Name = customerRequest.Name,
                    Address = customerRequest.Address
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                var customerDto = new CustomerDTO
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Address = customer.Address
                };

                return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customerDto);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while adding the customer. Please try again later.");
            }
        }

        // GET: api/Customers/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDTO>> GetCustomer(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid customer ID. ID must be greater than zero.");
            }

            try
            {
                var customer = await _context.Customers.FindAsync(id);

                if (customer == null)
                {
                    return NotFound();
                }

                var customerDto = new CustomerDTO
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Address = customer.Address
                };

                return customerDto;
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving the customer.");
            }
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> GetCustomers()
        {
            try
            {
                var customers = await _context.Customers.ToListAsync();

                var customerDtos = new List<CustomerDTO>();
                foreach (var customer in customers)
                {
                    customerDtos.Add(new CustomerDTO
                    {
                        Id = customer.Id,
                        Name = customer.Name,
                        Address = customer.Address
                    });
                }

                return customerDtos;
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving the customers list.");
            }
        }

        // GET: api/Customers/count
        [HttpGet("count")]
        public async Task<ActionResult<object>> GetCustomerCount()
        {
            try
            {
                var count = await _context.Customers.CountAsync();
                return Ok(new { count });
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while counting customers.");
            }
        }

        // PUT: api/Customers/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerRequest customerRequest)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid customer ID. ID must be greater than zero.");
            }

            if (customerRequest == null || string.IsNullOrWhiteSpace(customerRequest.Name) || string.IsNullOrWhiteSpace(customerRequest.Address))
            {
                return BadRequest("Customer details are required for update. Please ensure Name and Address are provided.");
            }

            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }

                customer.Name = customerRequest.Name;
                customer.Address = customerRequest.Address;

                _context.Entry(customer).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating the customer.");
            }
        }

        // DELETE: api/Customers/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid customer ID. ID must be greater than zero.");
            }

            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while deleting the customer.");
            }
        }
    }

    public class CustomerRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
