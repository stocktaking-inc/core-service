using CoreService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreService.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailRequest emailRequest);
        Task<EmailRequest> GetEmailRequestAsync(int id);
        Task<IEnumerable<EmailRequest>> GetAllEmailRequestsAsync(int page, int limit);
    }
}