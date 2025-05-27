using CoreService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreService.Interfaces
{
    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetAllAsync(int page, int limit);
        Task<Item> GetByIdAsync(int id);
        Task AddAsync(Item item);
        Task UpdateAsync(Item item);
        Task DeleteAsync(Item item);
    }
}