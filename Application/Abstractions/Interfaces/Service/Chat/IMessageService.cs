using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.Chat;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IMessageService
    {
        Task<Message> GetByIdAsync(int id);
        Task<IEnumerable<Message>> GetAllAsync();
        Task<Message> CreateAsync(Message message);
        Task<Message> UpdateAsync(Message message);
        Task<bool> DeleteAsync(int id);
    }
}

