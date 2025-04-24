using Microsoft.AspNetCore.Mvc;
using app.Server.Models;
using app.Server.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace app.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly StoreDbContext _context;

        public StoresController(StoreDbContext context)
        {
            _context = context;
        }

        // POST: api/Store
        [HttpPost(Name = "AddStore")]
        public async Task<ActionResult<StoreDTO>> AddStore([FromBody] StoreRequest storeRequest)
        {
            if (storeRequest == null || string.IsNullOrWhiteSpace(storeRequest.Name) || string.IsNullOrWhiteSpace(storeRequest.Address))
            {
                return BadRequest("Store name and address must be provided.");
            }

            try
            {
                var store = new Store
                {
                    Name = storeRequest.Name,
                    Address = storeRequest.Address
                };

                _context.Stores.Add(store);
                await _context.SaveChangesAsync();

                var storeDTO = new StoreDTO
                {
                    Id = store.Id,
                    Name = store.Name,
                    Address = store.Address
                };

                return CreatedAtAction(nameof(GetStore), new { id = store.Id }, storeDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the store: {ex.Message}");
            }
        }

        // GET: api/Store/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StoreDTO>> GetStore(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Store ID must be a positive integer.");
            }

            try
            {
                var store = await _context.Stores.FindAsync(id);
                if (store == null)
                {
                    return NotFound($"Store with ID {id} was not found.");
                }

                var storeDTO = new StoreDTO
                {
                    Id = store.Id,
                    Name = store.Name,
                    Address = store.Address
                };

                return storeDTO;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the store: {ex.Message}");
            }
        }

        // GET: api/Store
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StoreDTO>>> GetStores()
        {
            try
            {
                var stores = await _context.Stores.ToListAsync();
                if (stores.Count == 0)
                {
                    return NotFound("No stores found.");
                }

               
                var storeDTOs = stores.Select(store => new StoreDTO
                {
                    Id = store.Id,
                    Name = store.Name,
                    Address = store.Address
                }).ToList();

                return storeDTOs;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the stores: {ex.Message}");
            }
        }

        // PUT: api/Store/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStore(int id, [FromBody] StoreRequest storeRequest)
        {
            if (id <= 0)
            {
                return BadRequest("Store ID must be a positive integer.");
            }

            if (storeRequest == null || string.IsNullOrWhiteSpace(storeRequest.Name) || string.IsNullOrWhiteSpace(storeRequest.Address))
            {
                return BadRequest("Store name and address must be provided.");
            }

            try
            {
                var store = await _context.Stores.FindAsync(id);
                if (store == null)
                {
                    return NotFound($"Store with ID {id} does not exist.");
                }

                store.Name = storeRequest.Name;
                store.Address = storeRequest.Address;

                _context.Entry(store).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok($"Store with ID {id} has been updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the store: {ex.Message}");
            }
        }

        // DELETE: api/Store/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Store ID must be a positive integer.");
            }

            try
            {
                var store = await _context.Stores.FindAsync(id);
                if (store == null)
                {
                    return NotFound($"Store with ID {id} does not exist.");
                }

                _context.Stores.Remove(store);
                await _context.SaveChangesAsync();

                return Ok($"Store with ID {id} has been deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the store: {ex.Message}");
            }
        }
        // GET: api/Stores/count
        [HttpGet("count")]
        public async Task<ActionResult<object>> GetStoreCount()
        {
            try
            {
                var storeCount = await _context.Stores.CountAsync();
                return Ok(new { storeCount });
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while counting stores.");
            }
        }

    }

    public class StoreRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
