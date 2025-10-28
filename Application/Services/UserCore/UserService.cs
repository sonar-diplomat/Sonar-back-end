using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.DTOs.Auth;
using Application.DTOs.User;
using Application.Extensions;
using Application.Response;
using Entities.Models.Access;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;

namespace Application.Services.UserCore;

public class UserService(
    IUserRepository repository,
    IVisibilityStateService visibilityStateService,
    IInventoryService inventoryService,
    IAccessFeatureService accessFeatureService,
    ISettingsService settingsService,
    IUserStateService stateService,
    IImageFileService imageFileService
)
    : IUserService
{
    public async Task<int> ChangeCurrencyAsync(int userId, int modifier)
    {
        User user = await GetByIdAsync(userId);
        if (user == null)
            throw ResponseFactory.Create<NotFoundResponse>(["User not found."]);
        user.AvailableCurrency += modifier;
        return user.AvailableCurrency;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await repository.GetByIdAsync(id);
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

    public async Task ChangeUserNameAsync(int userId, string newUserName)
    {
        User user = await GetByIdAsync(userId);
        if (user.UserName == newUserName)
            throw ResponseFactory.Create<BadRequestResponse>(["UserName is already set to this value."]);
        if (await repository.IsUserNameTakenAsync(newUserName))
            throw ResponseFactory.Create<BadRequestResponse>(["UserName is already taken."]);
        user.UserName = newUserName;
        await repository.UpdateAsync(user);
    }

    public async Task<User> CreateUserShellAsync(UserRegisterDTO model)
    {
        User user = new()
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            DateOfBirth = model.DateOfBirth,
            UserName = model.UserName,
            Login = model.Login,
            Email = model.Email,
            PublicIdentifier = RandomNumberGenerator.GetInt32(100000, 1000000).ToString(), // TODO: Potentially change, needs discussion
            RegistrationDate = DateTime.UtcNow
        };
        VisibilityState tempVs = new()
        {
            SetPublicOn = DateTime.UtcNow,
            StatusId = 1
        };
        await visibilityStateService.CreateAsync(tempVs);
        user.VisibilityState = tempVs;
        user.Inventory = await inventoryService.CreateDefaultAsync();
        user.AccessFeatures = await accessFeatureService.GetDefaultAsync();
        user.Settings = await settingsService.CreateDefaultAsync(model.Locale);
        user.UserState = await stateService.CreateDefaultAsync();
        user.AvatarImage = await imageFileService.GetDefaultAsync();
        return user;
    }

    public async Task<User> GetByIdValidatedAsync(int id)
    {
        User? user = await repository.GetByIdAsync(id);
        return user ?? throw ResponseFactory.Create<NotFoundResponse>(["User not found."]);
    }

    public async Task<User> GetValidatedIncludeAccessFeaturesAsync(int id)
    {
        return await repository.Include(u => u.AccessFeatures).GetByIdValidatedAsync(id);
    }

    public async Task UpdateAvatar(int userId, IFormFile file)
    {
        User user = await GetByIdValidatedAsync(userId);
        int oldAvatarId = user.AvatarImageId;
        if (oldAvatarId != 1)
            await imageFileService.DeleteAsync(oldAvatarId);

        user.AvatarImage = await imageFileService.UploadFileAsync(file);
        await repository.UpdateAsync(user);
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        User? user = await GetByIdAsync(userId);
        if (user == null)
            throw ResponseFactory.Create<NotFoundResponse>();

        await repository.RemoveAsync(user);
        return true;
    }


    public async Task AssignAccessFeaturesAsync(int userId, int[] accessFeatureIds)
    {
        User user = await GetValidatedIncludeAccessFeaturesAsync(userId);
        foreach (int accessFeatureId in accessFeatureIds)
        {
            if (user.AccessFeatures.All(af => af.Id != accessFeatureId))
                user.AccessFeatures.Add(await accessFeatureService.GetByIdValidatedAsync(accessFeatureId));
        }
        await repository.SaveChangesAsync();
    }

    public async Task AssignAccessFeaturesByNameAsync(int userId, string[] accessFeatures)
    {
        User user = await GetValidatedIncludeAccessFeaturesAsync(userId);
        foreach (string name in accessFeatures)
        {
            if (user.AccessFeatures.All(af => af.Name != name))
                user.AccessFeatures.Add(await accessFeatureService.GetByNameValidatedAsync(name));
        }
        await repository.SaveChangesAsync();
    }

    public async Task RevokeAccessFeaturesAsync(int userId, int[] accessFeatureIds)
    {
        var user = await GetValidatedIncludeAccessFeaturesAsync(userId);
        IEnumerable<AccessFeature> toRemove = user.AccessFeatures.Where(af => accessFeatureIds.Contains(af.Id));
        foreach (AccessFeature af in toRemove)
            user.AccessFeatures.Remove(af);
        await repository.SaveChangesAsync();
    }

    public async Task RevokeAccessFeaturesByNameAsync(int userId, string[] accessFeatures)
    {
        var user = await GetValidatedIncludeAccessFeaturesAsync(userId);
        IEnumerable<AccessFeature> toRemove = user.AccessFeatures.Where(af => accessFeatures.Contains(af.Name));
        foreach (AccessFeature af in toRemove)
            user.AccessFeatures.Remove(af);
        await repository.SaveChangesAsync();
    }
}
