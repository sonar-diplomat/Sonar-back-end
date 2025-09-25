using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models;
using Entities.Models.User;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IUserStatusService
    {
        Task<UserStatus> GetByIdAsync(int id);
        Task<IEnumerable<UserStatus>> GetAllAsync();
        Task<UserStatus> CreateAsync(UserStatus status);
        Task<UserStatus> UpdateAsync(UserStatus status);
        Task<bool> DeleteAsync(int id);
    }
}
