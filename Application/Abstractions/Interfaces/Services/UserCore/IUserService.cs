using Application.DTOs.Auth;
using Application.DTOs.User;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Http;

namespace Application.Abstractions.Interfaces.Services;

public interface IUserService
{
    Task<int> ChangeCurrencyAsync(int userId, int modifier);
    Task<User> CreateUserShellAsync(UserRegisterDTO userRegisterDTO);
    Task<User> UpdateUserAsync(int userId, UserUpdateDTO userUpdateDto);
    Task<User> UpdateUserAsync(User user);
    Task ChangeUserNameAsync(int userId, string newUserName);
    Task<User?> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetByIdValidatedAsync(int id);
    Task<User> GetByPublicIdentifierValidatedAsync(string publicIdentifier);
    Task UpdateAvatar(int userId, IFormFile file);
    Task UpdateVisibilityStatusAsync(int collectionId, int newVisibilityStatusId);
    Task AssignAccessFeaturesAsync(int userId, int[] accessFeatureIds);
    Task AssignAccessFeaturesByNameAsync(int userId, string[] accessFeatures);
    Task RevokeAccessFeaturesAsync(int userId, int[] accessFeatureIds);
    Task RevokeAccessFeaturesByNameAsync(int userId, string[] accessFeatures);
    Task RemoveFriendAsync(int userId, int friendId);
    Task<IEnumerable<User>> GetFriendsAsync(int userId);
}