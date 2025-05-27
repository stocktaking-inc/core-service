using CoreService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreService.Interfaces
{

    public interface IInvoiceRepository
    {
        Task<Invoice> GetByIdAsync(int id);
        Task<IEnumerable<Invoice>> GetByOrderIdAsync(int orderId);
        Task AddAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);
    }
}