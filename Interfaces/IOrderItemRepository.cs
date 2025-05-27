using CoreService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreService.Interfaces
{

    public interface IOrderItemRepository
    {
        Task<OrderItem> GetByIdAsync(int id);
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId);
        Task AddAsync(OrderItem orderItem);
        Task UpdateAsync(OrderItem orderItem);
        Task DeleteAsync(OrderItem orderItem);
    }
}