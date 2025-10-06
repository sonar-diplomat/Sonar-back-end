using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Entities.Models.Access;
using Entities.Models.UserCore;
using Entities.Models.UserExperience;

namespace Application.Services.UserCore;

public class UserService(
    IUserRepository repository,
    IVisibilityStateService visibilityStateService,
    IInventoryService inventoryService,
    IAccessFeatureService accessFeatureService,
    ISettingsService settingsService,
    IUserStateService stateService,
    IFileService fileService
)
    : IUserService
{
    public async Task<int> ChangeCurrencyAsync(int userId, int modifier)
    {
        User user = await GetByIdAsync(userId);
        user.AvailableCurrency += modifier;
        return user.AvailableCurrency;
    }

    public async Task<User> GetByIdAsync(int id)
    {
        return (await repository.GetByIdAsync(id))!;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await repository.GetAllAsync();
    }

    // TODO: implement this pile of functions
    public async Task<bool> ChangeEmailAsync(int userId, string newEmail)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateUserAsync(int userId, UserDTO userUpdateDTO)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ChangePasswordAsync(int userId, string newPassword)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ChangeUsernameAsync(int userId, string newUsername)
    {
        throw new NotImplementedException();
    }

    public async Task<User> CreateUserShellAsync(UserRegisterDTO model)
    {
        User user = new()
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            DateOfBirth = model.DateOfBirth,
            Username = model.Username,
            Login = model.Login,
            Email = model.Email
        };
        VisibilityState tempVS = new()
        {
            SetPublicOn = DateTime.UtcNow,
            StatusId = 1
        };
        await visibilityStateService.CreateAsync(tempVS);
        user.VisibilityState = tempVS;
        Inventory tempI = new() { User = user };
        await inventoryService.CreateAsync(tempI);
        user.Inventory = tempI;
        user.AccessFeatures = await accessFeatureService.GetDefaultAsync();
        user.Settings = await settingsService.CreateDefaultAsync(model.Locale);
        user.UserState = await stateService.CreateDefaultAsync();
        user.AvatarImage = await fileService.GetDefaultAsync();
        return user;
    }
}