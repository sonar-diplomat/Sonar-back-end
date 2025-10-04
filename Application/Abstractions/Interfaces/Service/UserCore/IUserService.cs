using Application.Abstractions.Interfaces.Service;
using Application.DTOs;
using Entities.Models.UserCore;

namespace Application.Abstractions.Interfaces.Services
{
    public interface IUserService : IGenericService<User>
    {
        Task<User> CreateUserAsync(UserRegisterDTO userRegisterDTO);
        Task<bool> UpdateUserAsync(int userId, UserDTO userUpdateDTO);
        Task<bool> ChangePasswordAsync(int userId, string newPassword);
        Task<bool> ChangeUsernameAsync(int userId, string newUsername);
        Task<bool> ChangeEmailAsync(int userId, string newEmail);
        Task<int> ChangeCurrencyAsync(int userId, int modifier);

        // TODO: implement method
        // Task<bool> connectAccount(int userId);
        Task<bool> Manage2FAAsync(int userId);
        Task<bool> Verify2FAAsync(int userId, string token);
    }
}