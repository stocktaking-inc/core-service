using CoreService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreService.Interfaces
{

    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync(int page, int limit);
        Task<Order> GetByIdAsync(int id);
        Task<Order> GetByIdWithDetailsAsync(int id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Order order);
        Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId);
    }
}