using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Application.Abstractions.Interfaces.Services.File;
using Application.Abstractions.Interfaces.Services.UserCore;
using Application.DTOs.Auth;
using Application.DTOs.User;
using Application.Extensions;
using Application.Response;
using Entities.Models.Access;
using Entities.Models.File;
using Entities.Models.Music;
using Entities.Models.UserCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Application.Services.UserCore;

public class UserService(
    IUserRepository repository,
    IVisibilityStateService visibilityStateService,
    IInventoryService inventoryService,
    IAccessFeatureService accessFeatureService,
    ISettingsService settingsService,
    IUserStateService stateService,
    IImageFileService imageFileService,
    ILibraryService libraryService,
    IUserFollowService userFollowService
)
    : IUserService
{
    private const string StartIdentifierValue = "3";

    public async Task<int> ChangeCurrencyAsync(int userId, int modifier)
    {
        User user = await GetByIdValidatedAsync(userId);
        user.AvailableCurrency += modifier;
        return user.AvailableCurrency;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await repository.SnInclude(u => u.AccessFeatures).GetAllAsync();
    }

    public async Task<User> UpdateUserAsync(int userId, UserUpdateDTO userUpdateDto)
    {
        User user = await GetByIdValidatedAsync(userId);
        if (userUpdateDto.PublicIdentifier is not null)
        {
            if ((await GetAllAsync()).Select(u => u.PublicIdentifier).Any(pi => pi == userUpdateDto.PublicIdentifier))
                throw ResponseFactory.Create<BadRequestResponse>(["PublicIdentifier is already taken."]);
            user.PublicIdentifier = userUpdateDto.PublicIdentifier;
        }
        if (userUpdateDto.Biography is not null)
            user.Biography = userUpdateDto.Biography;
        if (userUpdateDto.DateOfBirth is not null)
            user.DateOfBirth = (DateOnly)userUpdateDto.DateOfBirth;
        if (userUpdateDto.LastName is not null)
            user.LastName = userUpdateDto.LastName;
        if (userUpdateDto.FirstName is not null)
            user.FirstName = userUpdateDto.FirstName;

        return await repository.UpdateAsync(user);
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        return await repository.UpdateAsync(user);
    }

    public async Task ChangeUserNameAsync(int userId, string newUserName)
    {
        User user = await GetByIdValidatedAsync(userId);
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
            PublicIdentifier = await GenerateUniqueUserPublicIdentifierAsync(),
            RegistrationDate = DateTime.UtcNow
        };
        VisibilityState tempVs = new()
        {
            SetPublicOn = DateTime.UtcNow,
            StatusId = 1
        };

        Playlist playlist = new()
        {
            Name = "Favorites",
            VisibilityState = await visibilityStateService.CreateDefaultAsync(),
            Cover = await imageFileService.GetDefaultAsync(),
            Creator = user
        };

        await visibilityStateService.CreateAsync(tempVs);
        user.VisibilityState = tempVs;
        user.Inventory = await inventoryService.CreateDefaultAsync();
        user.AccessFeatures = await accessFeatureService.GetDefaultAsync();
        user.Settings = await settingsService.CreateDefaultAsync(model.Locale);
        user.UserState = await stateService.CreateDefaultAsync();
        user.AvatarImage = await imageFileService.GetDefaultAsync();
        user.Library = await libraryService.CreateDefaultAsync();
        user.Library.RootFolder!.Collections.Add(playlist);
        return user;
    }

    public async Task<User> GetByIdValidatedAsync(int id)
    {
        User? user = await repository.GetByIdAsync(id);
        return user ?? throw ResponseFactory.Create<NotFoundResponse>(["User not found."]);
    }

    public async Task<User> GetByPublicIdentifierValidatedAsync(string publicIdentifier)
    {
        User? user = await repository.GetByPublicIdentifierAsync(publicIdentifier);
        return user ?? throw ResponseFactory.Create<NotFoundResponse>(["User not found."]);
    }

    public async Task UpdateAvatar(int userId, IFormFile file)
    {
        User user = await GetByIdValidatedAsync(userId);
        int oldAvatarId = user.AvatarImageId;

        ImageFile newAvatar = await imageFileService.UploadFileAsync(file);
        await repository.UpdateAvatarImageIdAsync(userId, newAvatar.Id);

        if (oldAvatarId != 1)
            await imageFileService.DeleteAsync(oldAvatarId);
    }

    public async Task AssignAccessFeaturesAsync(int userId, int[] accessFeatureIds)
    {
        User user = await GetValidatedIncludeAccessFeaturesAsync(userId);
        foreach (int accessFeatureId in accessFeatureIds)
            if (user.AccessFeatures.All(af => af.Id != accessFeatureId) && accessFeatureId != 1 /*IAmGod protection*/)
                user.AccessFeatures.Add(await accessFeatureService.GetByIdValidatedAsync(accessFeatureId));

        await repository.UpdateAsync(user);
    }

    public async Task AssignAccessFeaturesByNameAsync(int userId, string[] accessFeatures)
    {
        User user = await GetValidatedIncludeAccessFeaturesAsync(userId);
        foreach (string name in accessFeatures)
            if (user.AccessFeatures.All(af => af.Name != name))
                user.AccessFeatures.Add(await accessFeatureService.GetByNameValidatedAsync(name));

        await repository.UpdateAsync(user);
    }

    public async Task RevokeAccessFeaturesAsync(int userId, int[] accessFeatureIds)
    {
        User user = await GetValidatedIncludeAccessFeaturesAsync(userId);
        IEnumerable<AccessFeature> toRemove = user.AccessFeatures.Where(af => accessFeatureIds.Contains(af.Id));
        foreach (AccessFeature af in toRemove)
        {
            if (af.Id != 1 && af.Name != "IAmGod" /*protection for IAmGod*/) user.AccessFeatures.Remove(af);
        }

        await repository.UpdateAsync(user);
    }

    public async Task RevokeAccessFeaturesByNameAsync(int userId, string[] accessFeatures)
    {
        User user = await GetValidatedIncludeAccessFeaturesAsync(userId);
        IEnumerable<AccessFeature> toRemove = user.AccessFeatures.Where(af => accessFeatures.Contains(af.Name));
        foreach (AccessFeature af in toRemove)
            user.AccessFeatures.Remove(af);
        await repository.UpdateAsync(user);
    }

    public async Task UpdateVisibilityStatusAsync(int userId, int newVisibilityStatusId)
    {
        User user = await repository.SnInclude(a => a.VisibilityState).GetByIdValidatedAsync(userId);
        user.VisibilityState.StatusId = newVisibilityStatusId;
        await repository.UpdateAsync(user);
    }

    private async Task<User> GetValidatedIncludeAccessFeaturesAsync(int id)
    {
        return await repository.SnInclude(u => u.AccessFeatures).GetByIdValidatedAsync(id);
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        User user = await GetByIdValidatedAsync(userId);
        await repository.RemoveAsync(user);
        return true;
    }


    public async Task RemoveFriendAsync(int userId, int friendId)
    {
        User user = await repository.SnInclude(u => u.Friends).GetByIdValidatedAsync(userId);
        User friend = await repository.SnInclude(u => u.Friends).GetByIdValidatedAsync(friendId);

        if (user.Friends.All(f => f.Id != friendId))
            throw ResponseFactory.Create<BadRequestResponse>(["This user is not in your friends list."]);

        user.Friends.Remove(friend);
        friend.Friends.Remove(user);

        await repository.UpdateAsync(user);
        await repository.UpdateAsync(friend);
    }

    public async Task<IEnumerable<User>> GetFriendsAsync(int userId)
    {
        return await userFollowService.GetMutualFollowsAsync(userId);
    }

    public async Task RegisterUserWithTransactionAsync(
        UserRegisterDTO model,
        UserManager<User> userManager,
        Func<User, Task> sendConfirmationEmailAsync)
    {
        await using var transaction = await repository.BeginTransactionAsync();
        bool rolledBack = false;

        try
        {
            User user = await CreateUserShellAsync(model);
            IdentityResult result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                await transaction.RollbackAsync();
                rolledBack = true;
                ThrowIdentityErrors(result, userManager);
            }

            try
            {
                await sendConfirmationEmailAsync(user);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                rolledBack = true;
                throw ResponseFactory.Create<ExpectationFailedResponse>([
                    "Failed to send confirmation email. Please try again later."
                ]);
            }

            await transaction.CommitAsync();
        }
        catch (DbUpdateException dbEx) when (dbEx.InnerException?.Message.Contains("duplicate") == true ||
                                             dbEx.InnerException?.Message.Contains("unique") == true ||
                                             dbEx.InnerException?.Message.Contains("UserNameIndex") == true ||
                                             dbEx.InnerException?.Message.Contains("EmailIndex") == true)
        {
            if (!rolledBack)
            {
                await transaction.RollbackAsync();
                rolledBack = true;
            }
            string errorMessage = dbEx.InnerException?.Message ?? dbEx.Message;

            if (errorMessage.Contains("UserName") || errorMessage.Contains("UserNameIndex"))
                throw ResponseFactory.Create<ConflictResponse>(["User with this username already exists"]);

            if (errorMessage.Contains("Email") || errorMessage.Contains("EmailIndex"))
                throw ResponseFactory.Create<ConflictResponse>(["Email is already in use"]);

            throw ResponseFactory.Create<ConflictResponse>(["User with such data already exists"]);
        }
        catch (Exception ex)
        {
            if (!rolledBack)
            {
                await transaction.RollbackAsync();
                rolledBack = true;
            }
            throw ResponseFactory.Create<ExpectationFailedResponse>([
                //"Registration could not be completed due to an error. Please try again later."
                ex.Message
            ]);
        }
    }

    private static void ThrowIdentityErrors(IdentityResult result, UserManager<User> userManager)
    {
        List<string> conflictErrors = new();
        List<string> identityValidationErrors = new();

        foreach (IdentityError error in result.Errors)
        {
            string message = error.Code switch
            {
                "DuplicateUserName" or "DuplicateNormalizedUserName" => "User with this username already exists",
                "DuplicateEmail" => "Email is already in use",
                "PasswordTooShort" => $"Password is too short. Minimum length: {userManager.Options.Password.RequiredLength} characters",
                "PasswordRequiresNonAlphanumeric" => "Password must contain at least one special character",
                "PasswordRequiresDigit" => "Password must contain at least one digit",
                "PasswordRequiresLower" => "Password must contain at least one lowercase letter",
                "PasswordRequiresUpper" => "Password must contain at least one uppercase letter",
                "InvalidEmail" => "Invalid email address",
                "InvalidUserName" => "Invalid username",
                _ => error.Description
            };

            if (error.Code is "DuplicateUserName" or "DuplicateNormalizedUserName" or "DuplicateEmail")
                conflictErrors.Add(message);
            else
                identityValidationErrors.Add(message);
        }

        if (conflictErrors.Count > 0)
            throw ResponseFactory.Create<ConflictResponse>(conflictErrors.ToArray());

        if (identityValidationErrors.Count > 0)
            throw ResponseFactory.Create<BadRequestResponse>(identityValidationErrors.ToArray());

        throw ResponseFactory.Create<BadRequestResponse>(
            result.Errors.Select(e => e.Description).ToArray());
    }

    private async Task<string> GenerateUniqueUserPublicIdentifierAsync()
    {
        while (true)
        {
            string publicIdentifier = string.Concat(StartIdentifierValue,
                RandomNumberGenerator.GetInt32(100000, 1000000).ToString());

            if (!await repository.CheckExists(publicIdentifier)) return publicIdentifier;
        }
    }
}