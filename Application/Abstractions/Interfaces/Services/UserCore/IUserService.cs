using Application.DTOs.Auth;
using Application.DTOs.User;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Http;

namespace Application.Abstractions.Interfaces.Services;

public interface IUserService
{
    Task<int> ChangeCurrencyAsync(int userId, int modifier);
    Task<User> CreateUserShellAsync(UserRegisterDTO userRegisterDTO);
    Task<User> UpdateUserAsync(int userId, UserUpdateDTO userUpdateUpdateDto);
    Task<User> UpdateUserAsync(User user);
    Task ChangeUserNameAsync(int userId, string newUserName);
    Task<User> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetByIdValidatedAsync(int id);
    Task UpdateAvatar(int userId, IFormFile file);
    Task<User> GetValidatedIncludeAccessFeaturesAsync(int id);
}
