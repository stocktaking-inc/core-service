using CoreService.DTOs;
using CoreService.Models;
using CoreService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly AuthDbContext _context;

        public SuppliersController(AuthDbContext context)
        {
            _context = context;
        }

        // GET: api/suppliers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierDTO>>> GetSuppliers()
        {
          return await _context.Suppliers
            .Include(s => s.Goods) // Подгружаем связанные товары
            .Select(s => new SupplierDTO
            {
              SupplierId = s.Id,
              Name = s.Name,
              ContactPerson = s.ContactPerson,
              Email = s.Email,
              Phone = s.Phone,
              Status = s.Status,
              Goods = s.Goods.Select(g => new GoodDTO
              {
                Id = g.Id,
                Name = g.Name,
                Article = g.Article,
                PurchasePrice = g.PurchasePrice,
                Category = g.Category,
                SupplierId = g.SupplierId
              }).ToList()
            })
            .ToListAsync();
        }

        // GET: api/suppliers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierDTO>> GetSupplier(int id)
        {
          var supplier = await _context.Suppliers
            .Include(s => s.Goods) // Подгружаем связанные товары
            .FirstOrDefaultAsync(s => s.Id == id);

          if (supplier == null)
          {
            return NotFound(new { message = "Поставщик не найден" });
          }

          return new SupplierDTO
          {
            SupplierId = supplier.Id,
            Name = supplier.Name,
            ContactPerson = supplier.ContactPerson,
            Email = supplier.Email,
            Phone = supplier.Phone,
            Status = supplier.Status,
            Goods = supplier.Goods.Select(g => new GoodDTO
            {
              Id = g.Id,
              Name = g.Name,
              Article = g.Article,
              PurchasePrice = g.PurchasePrice,
              Category = g.Category,
              SupplierId = g.SupplierId
            }).ToList()
          };
        }

        // POST: api/suppliers
        [HttpPost]
        public async Task<ActionResult<SupplierDTO>> PostSupplier([FromBody] SupplierDTO supplierDTO)
        {
            if (supplierDTO == null)
            {
                return BadRequest(new { message = "SupplierDTO is required." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var supplier = new Supplier
            {
                Name = supplierDTO.Name ?? throw new ArgumentException("Name is required"),
                ContactPerson = supplierDTO.ContactPerson,
                Email = supplierDTO.Email,
                Phone = supplierDTO.Phone,
                Status = supplierDTO.Status
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            supplierDTO.SupplierId = supplier.Id;
            return CreatedAtAction(nameof(GetSupplier), new { id = supplier.Id }, supplierDTO);
        }

        // PUT: api/suppliers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSupplier(int id, SupplierDTO supplierDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound(new { message = "Поставщик не найден" });
            }

            supplier.Name = supplierDTO.Name ?? throw new ArgumentException("Name is required");
            supplier.ContactPerson = supplierDTO.ContactPerson;
            supplier.Email = supplierDTO.Email;
            supplier.Phone = supplierDTO.Phone;
            supplier.Status = supplierDTO.Status;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(id))
                {
                    return NotFound(new { message = "Поставщик не найден" });
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/suppliers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound(new { message = "Поставщик не найден" });
            }

            if (await _context.Items.AnyAsync(i => i.SupplierId == id))
            {
                return BadRequest(new { message = "Нельзя удалить поставщика, так как он связан с элементами в таблице items." });
            }

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/suppliers/{supplierId}/goods
        [HttpPost("{supplierId}/goods")]
        public async Task<ActionResult<GoodDTO>> AddGoodToSupplier(int supplierId, [FromBody] GoodDTO goodDTO)
        {
          if (!ModelState.IsValid)
          {
            return BadRequest(ModelState);
          }

          // Проверяем существование поставщика
          if (!await _context.Suppliers.AnyAsync(s => s.Id == supplierId))
          {
            return NotFound(new { message = "Поставщик не найден" });
          }

          // Проверяем уникальность артикула
          if (await _context.Goods.AnyAsync(g => g.Article == goodDTO.Article))
          {
            return BadRequest(new { message = "Товар с таким артикулом уже существует" });
          }

          var good = new Good
          {
            Name = goodDTO.Name ?? throw new ArgumentException("Name is required"),
            Article = goodDTO.Article ?? throw new ArgumentException("Article is required"),
            PurchasePrice = goodDTO.PurchasePrice,
            Category = goodDTO.Category,
            SupplierId = supplierId
          };

          _context.Goods.Add(good);
          await _context.SaveChangesAsync();

          goodDTO.Id = good.Id;
          return CreatedAtAction(nameof(GetGood), new { id = good.Id }, goodDTO);
        }

        // GET: api/goods/{id}
        [HttpGet("goods/{id}")]
        public async Task<ActionResult<GoodDTO>> GetGood(int id)
        {
          var good = await _context.Goods.FindAsync(id);
          if (good == null)
          {
            return NotFound(new { message = "Товар не найден" });
          }

          return new GoodDTO
          {
            Id = good.Id,
            Name = good.Name,
            Article = good.Article,
            PurchasePrice = good.PurchasePrice,
            Category = good.Category,
            SupplierId = good.SupplierId
          };
        }

        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.Id == id);
        }

        private bool GoodExists(int id)
        {
          return _context.Goods.Any(e => e.Id == id);
        }

        private static SupplierDTO SupplierToDTO(Supplier supplier) =>
            new SupplierDTO
            {
                SupplierId = supplier.Id,
                Name = supplier.Name,
                ContactPerson = supplier.ContactPerson,
                Email = supplier.Email,
                Phone = supplier.Phone,
                Status = supplier.Status
            };
    }
}
