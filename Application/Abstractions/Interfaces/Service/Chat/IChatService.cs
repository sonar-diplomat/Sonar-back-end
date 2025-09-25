using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.Chat;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IChatService
    {
        Task<Chat> GetByIdAsync(int id);
        Task<IEnumerable<Chat>> GetAllAsync();
        Task<Chat> CreateAsync(Chat chat);
        Task<Chat> UpdateAsync(Chat chat);
        Task<bool> DeleteAsync(int id);
    }
}