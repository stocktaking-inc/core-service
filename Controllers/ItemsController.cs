using CoreService.DTOs;
using CoreService.Models;
using CoreService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly AuthDbContext _context;

        public ItemsController(AuthDbContext context)
        {
            _context = context;
        }

        // GET: api/items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDTO>>> GetItems()
        {
            return await _context.Items
                .Select(x => new ItemDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    Article = x.Article,
                    Category = x.Category,
                    Quantity = x.Quantity,
                    LocationId = x.LocationId,
                    Status = x.Status,
                    SupplierId = x.SupplierId
                })
                .ToListAsync();
        }

        // GET: api/items/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDTO>> GetItem(int id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return ItemToDTO(item);
        }

        // POST: api/items
        [HttpPost]
        public async Task<ActionResult<ItemDTO>> PostItem(ItemDTO itemDTO)
        {
            var item = new Item
            {
                Name = itemDTO.Name,
                Article = itemDTO.Article,
                Category = itemDTO.Category,
                Quantity = itemDTO.Quantity,
                LocationId = itemDTO.LocationId,  // Now accepts nullable
                Status = itemDTO.Status,
                SupplierId = itemDTO.SupplierId
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, ItemToDTO(item));
        }

        // PUT: api/items/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(int id, ItemDTO itemDTO)
        {
            if (id != itemDTO.Id)
            {
                return BadRequest();
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            item.Name = itemDTO.Name;
            item.Article = itemDTO.Article;
            item.Category = itemDTO.Category;
            item.Quantity = itemDTO.Quantity;
            item.LocationId = itemDTO.LocationId;  // Now handles nullable
            item.Status = itemDTO.Status;
            item.SupplierId = itemDTO.SupplierId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/items/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }

        private static ItemDTO ItemToDTO(Item item) =>
            new()
            {
                Id = item.Id,
                Name = item.Name,
                Article = item.Article,
                Category = item.Category,
                Quantity = item.Quantity,
                LocationId = item.LocationId,  // Now handles nullable
                Status = item.Status,
                SupplierId = item.SupplierId
            };
    }
}