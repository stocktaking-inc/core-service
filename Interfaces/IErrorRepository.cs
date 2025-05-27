using CoreService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreService.Interfaces
{
    public interface IErrorRepository
    {
        Task<IEnumerable<Error>> GetAllAsync(int page, int limit);
        Task<Error> GetByIdAsync(int id);
        Task AddAsync(Error error);
    }
}