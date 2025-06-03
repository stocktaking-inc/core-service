using CoreService.DTOs;
using CoreService.Models;
using CoreService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly AuthDbContext _context;

        public WarehousesController(AuthDbContext context)
        {
            _context = context;
        }

        // GET: api/warehouses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WarehouseDTO>>> GetWarehouses()
        {
            return await _context.Warehouse
                .Select(x => new WarehouseDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                    IsActive = x.IsActive
                })
                .ToListAsync();
        }

        // GET: api/warehouses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WarehouseDTO>> GetWarehouse(int id)
        {
            var warehouse = await _context.Warehouse.FindAsync(id);

            if (warehouse == null)
            {
                return NotFound();
            }

            return WarehouseToDTO(warehouse);
        }

        // POST: api/warehouses
        [HttpPost]
        public async Task<ActionResult<WarehouseDTO>> PostWarehouse(WarehouseDTO warehouseDTO)
        {
            var warehouse = new Warehouse
            {
                Name = warehouseDTO.Name,
                Address = warehouseDTO.Address,
                IsActive = warehouseDTO.IsActive
            };

            _context.Warehouse.Add(warehouse);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWarehouse), new { id = warehouse.Id }, WarehouseToDTO(warehouse));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutWarehouse(int id, WarehouseDTO warehouseDTO)
        {
            var warehouse = await _context.Warehouse.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }

            warehouse.Name = warehouseDTO.Name;
            warehouse.Address = warehouseDTO.Address;
            warehouse.IsActive = warehouseDTO.IsActive;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WarehouseExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchWarehouse(int id, WarehouseDTO warehouseDTO)
        {
            var warehouse = await _context.Warehouse.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }

            if (warehouseDTO.IsActive != warehouse.IsActive)
            {
                warehouse.IsActive = warehouseDTO.IsActive;
            }
            else
            {
                return BadRequest("Не указано поле для обновления или значение isActive не изменилось.");
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WarehouseExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/warehouses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            var warehouse = await _context.Warehouse.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }

            if (await _context.Items.AnyAsync(i => i.LocationId == id))
            {
                return BadRequest("Нельзя удалить склад, так как он связан с элементами в таблице items.");
            }

            _context.Warehouse.Remove(warehouse);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WarehouseExists(int id)
        {
            return _context.Warehouse.Any(e => e.Id == id);
        }

        private static WarehouseDTO WarehouseToDTO(Warehouse warehouse) =>
            new()
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Address = warehouse.Address,
                IsActive = warehouse.IsActive
            };
    }
}
