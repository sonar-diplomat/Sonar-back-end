using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.Chat;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IMessageReadService
    {
        Task<MessageRead> GetByIdAsync(int id);
        Task<IEnumerable<MessageRead>> GetAllAsync();
        Task<MessageRead> CreateAsync(MessageRead messageRead);
        Task<MessageRead> UpdateAsync(MessageRead messageRead);
        Task<bool> DeleteAsync(int id);
    }
}

