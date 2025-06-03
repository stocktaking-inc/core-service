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
                .Select(x => new SupplierDTO
                {
                    SupplierId = x.Id,
                    Name = x.Name,
                    ContactPerson = x.ContactPerson,
                    Email = x.Email,
                    Phone = x.Phone,
                    Status = x.Status
                })
                .ToListAsync();
        }

        // GET: api/suppliers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierDTO>> GetSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound(new { message = "Поставщик не найден" });
            }
            return SupplierToDTO(supplier);
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

        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.Id == id);
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
