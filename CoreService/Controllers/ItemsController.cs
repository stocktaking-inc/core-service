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
        private readonly ILogger<ItemsController> _logger;

        public ItemsController(AuthDbContext context, ILogger<ItemsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDTO>>> GetItems()
        {
            return await _context.Items
                .AsNoTracking()
                .Select(i => new Item
                {
                    Id = i.Id,
                    Name = i.Name,
                    Article = i.Article,
                    Category = i.Category,
                    Quantity = i.Quantity,
                    LocationId = i.LocationId,
                    SupplierId = i.SupplierId,
                    Status = i.Status
                })
                .Select(x => ItemToDTO(x))
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDTO>> GetItem(int id)
        {
            var item = await _context.Items
                .AsNoTracking()
                .Where(i => i.Id == id)
                .Select(i => new Item
                {
                    Id = i.Id,
                    Name = i.Name,
                    Article = i.Article,
                    Category = i.Category,
                    Quantity = i.Quantity,
                    LocationId = i.LocationId,
                    SupplierId = i.SupplierId,
                    Status = i.Status
                })
                .FirstOrDefaultAsync();

            if (item == null)
                return NotFound();

            return ItemToDTO(item);
        }

        [HttpPost]
        public async Task<ActionResult<ItemDTO>> PostItem(ItemDTO itemDTO)
        {
            try
            {
                if (itemDTO.Quantity < 0)
                {
                    return BadRequest("Количество не может быть отрицательным.");
                }

                if (itemDTO.LocationId.HasValue && !await _context.Warehouse.AnyAsync(w => w.Id == itemDTO.LocationId))
                {
                    return BadRequest("Недопустимый LocationId: склад не существует.");
                }

                if (!await _context.Suppliers.AnyAsync(s => s.Id == itemDTO.SupplierId))
                {
                    return BadRequest("Недопустимый SupplierId: поставщик не существует.");
                }

                if (await _context.Items.AnyAsync(i => i.Article == itemDTO.Article))
                {
                    return BadRequest($"Артикул '{itemDTO.Article}' уже существует.");
                }

                var item = new Item
                {
                    Name = itemDTO.Name,
                    Article = itemDTO.Article,
                    Category = itemDTO.Category,
                    Quantity = itemDTO.Quantity,
                    LocationId = itemDTO.LocationId,
                    SupplierId = itemDTO.SupplierId
                };

                _context.Items.Add(item);
                await _context.SaveChangesAsync();

                var savedItem = await _context.Items
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.Id == item.Id);

                return CreatedAtAction(nameof(GetItem), new { id = item.Id }, ItemToDTO(savedItem ?? item));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении элемента с артикулом {Article}. Внутренняя ошибка: {InnerException}", itemDTO.Article, ex.InnerException?.Message);
                if (ex.InnerException?.Message.Contains("items_location_fkey") == true)
                {
                    return BadRequest("Недопустимый LocationId: склад не существует.");
                }
                if (ex.InnerException?.Message.Contains("items_supplier_fkey") == true)
                {
                    return BadRequest("Недопустимый SupplierId: поставщик не существует.");
                }
                if (ex.InnerException?.Message.Contains("items_pkey") == true)
                {
                    return BadRequest("Ошибка: конфликт первичного ключа. Последовательность items_id_seq несинхронизирована. Свяжитесь с администратором базы данных.");
                }
                return BadRequest($"Ошибка сохранения: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(int id, ItemDTO itemDTO)
        {
            if (itemDTO.Quantity < 0)
            {
                return BadRequest("Количество не может быть отрицательным.");
            }

            if (itemDTO.LocationId.HasValue && !await _context.Warehouse.AnyAsync(w => w.Id == itemDTO.LocationId))
            {
                return BadRequest("Недопустимый LocationId: склад не существует.");
            }

            if (!await _context.Suppliers.AnyAsync(s => s.Id == itemDTO.SupplierId))
            {
                return BadRequest("Недопустимый SupplierId: поставщик не существует.");
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            if (item.Article != itemDTO.Article && await _context.Items.AnyAsync(i => i.Article == itemDTO.Article))
            {
                return BadRequest($"Артикул '{itemDTO.Article}' уже существует.");
            }

            item.Name = itemDTO.Name;
            item.Article = itemDTO.Article;
            item.Category = itemDTO.Category;
            item.Quantity = itemDTO.Quantity;
            item.LocationId = itemDTO.LocationId;
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
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении элемента с артикулом {Article}", itemDTO.Article);
                if (ex.InnerException?.Message.Contains("items_article_key") == true)
                {
                    return BadRequest($"Артикул '{itemDTO.Article}' уже существует.");
                }
                if (ex.InnerException?.Message.Contains("items_location_fkey") == true)
                {
                    return BadRequest("Недопустимый LocationId: склад не существует.");
                }
                if (ex.InnerException?.Message.Contains("items_supplier_fkey") == true)
                {
                    return BadRequest("Недопустимый SupplierId: поставщик не существует.");
                }
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

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
                LocationId = item.LocationId,
                Status = item.Status == Item.StatusType.InStock ? "In Stock" :
                         item.Status == Item.StatusType.OutOfStock ? "Out of Stock" :
                         item.Status == Item.StatusType.LowStock ? "Low Stock" : "Out of Stock",
                SupplierId = item.SupplierId
            };
    }
}
