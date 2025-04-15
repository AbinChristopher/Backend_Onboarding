using Microsoft.AspNetCore.Mvc;
using app.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        // POST: api/Stores
        [HttpPost(Name = "AddStore")]
        public async Task<ActionResult<Store>> AddStore([FromBody] StoreRequest storeRequest)
        {
            var store = new Store
            {
                Name = storeRequest.Name,
                Address = storeRequest.Address
            };

            _context.Stores.Add(store);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStore), new { id = store.Id }, store);
        }

        // GET: api/Stores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Store>> GetStore(int id)
        {
            var store = await _context.Stores.FindAsync(id);

            if (store == null)
            {
                return NotFound();
            }

            return store;
        }

        // GET: api/Stores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Store>>> GetStores()
        {
            return await _context.Stores.ToListAsync();
        }

        // PUT: api/Stores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStore(int id, [FromBody] StoreRequest storeRequest)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            store.Name = storeRequest.Name;
            store.Address = storeRequest.Address;

            _context.Entry(store).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Stores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(int id)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    // Request body for adding or updating a store
    public class StoreRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
