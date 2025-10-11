using Application.DTOs;
using Entities.Models.UserCore;

namespace Application.Abstractions.Interfaces.Services;

public interface IUserService
{
    Task<int> ChangeCurrencyAsync(int userId, int modifier);
    Task<User> CreateUserShellAsync(UserRegisterDTO userRegisterDTO);
    Task<User> UpdateUserAsync(int userId, UserUpdateDTO userUpdateUpdateDto);
    Task ChangeUsernameAsync(int userId, string newUsername);

    Task<User> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetAllAsync();
}
