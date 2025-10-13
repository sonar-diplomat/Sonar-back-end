using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Exception;
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
        if (user == null) // TODO: create a custom exception for user without creating the dictionary object
            throw AppExceptionFactory.Create<NotFoundException>(["User not found."]);
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

    public async Task<User> UpdateUserAsync(int userId, UserUpdateDTO userUpdateUpdateDto)
    {
        User user = await GetByIdAsync(userId);
        if (userUpdateUpdateDto.PublicIdentifier is not null)
            user.PublicIdentifier = userUpdateUpdateDto.PublicIdentifier;
        if (userUpdateUpdateDto.Biography is not null)
            user.Biography = userUpdateUpdateDto.Biography;
        if (userUpdateUpdateDto.DateOfBirth is not null)
            user.DateOfBirth = (DateOnly)userUpdateUpdateDto.DateOfBirth;
        if (userUpdateUpdateDto.LastName is not null)
            user.LastName = userUpdateUpdateDto.LastName;
        if (userUpdateUpdateDto.FirstName is not null)
            user.FirstName = userUpdateUpdateDto.FirstName;

        return await repository.UpdateAsync(user);
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        return await repository.UpdateAsync(user);
    }

    public async Task ChangeUsernameAsync(int userId, string newUsername)
    {
        User user = await GetByIdAsync(userId);
        if (user.Username == newUsername)
            throw AppExceptionFactory.Create<BadRequestException>(["Username is already set to this value."]);
        if (await repository.IsUsernameTakenAsync(newUsername))
            throw AppExceptionFactory.Create<BadRequestException>(["Username is already taken."]);
        user.Username = newUsername;
        await repository.UpdateAsync(user);
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
        VisibilityState tempVs = new()
        {
            SetPublicOn = DateTime.UtcNow,
            StatusId = 1
        };
        await visibilityStateService.CreateAsync(tempVs);
        user.VisibilityState = tempVs;
        Inventory tempI = new() { User = user };
        await inventoryService.CreateAsync(tempI);
        user.Inventory = tempI;
        user.AccessFeatures = await accessFeatureService.GetDefaultAsync();
        user.Settings = await settingsService.CreateDefaultAsync(model.Locale);
        user.UserState = await stateService.CreateDefaultAsync();
        user.AvatarImage = await fileService.GetDefaultAsync();
        return user;
    }

    public async Task<User> GetByIdValidatedAsync(int id)
    {
        User? user = await repository.GetByIdAsync(id);
        return user ?? throw AppExceptionFactory.Create<NotFoundException>(["User not found."]);
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        User? user = await GetByIdAsync(userId);
        if (user == null)
            throw AppExceptionFactory.Create<NotFoundException>();

        await repository.RemoveAsync(user);
        return true;
    }
}
