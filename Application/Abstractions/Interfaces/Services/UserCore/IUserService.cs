using Application.DTOs;
using Entities.Models.UserCore;

namespace Application.Abstractions.Interfaces.Services;

public interface IUserService
{
    Task<User> CreateUserShellAsync(UserRegisterDTO userRegisterDTO);
    Task<bool> UpdateUserAsync(int userId, UserDTO userUpdateDTO);
    Task<bool> ChangePasswordAsync(int userId, string newPassword);
    Task<bool> ChangeUsernameAsync(int userId, string newUsername);
    Task<bool> ChangeEmailAsync(int userId, string newEmail);
    Task<int> ChangeCurrencyAsync(int userId, int modifier);
    Task<User> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetAllAsync();
}
