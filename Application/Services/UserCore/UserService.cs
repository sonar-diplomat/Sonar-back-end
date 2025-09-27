using Application.Abstractions.Interfaces.Repository.UserCore;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Application.Services.Access;
using Entities.Enums;
using Entities.Models.Access;
using Entities.Models.UserCore;
using Entities.Models.UserExperience;

namespace Application.Services.UserCore
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IVisibilityStateService visibilityStateService;
        private readonly IInventoryService inventoryService;

        public UserService(IUserRepository userRepository, 
            IVisibilityStateService visibilityStateService, 
            IInventoryService inventoryService)
        {
            this.userRepository = userRepository;
            this.visibilityStateService = visibilityStateService;
            this.inventoryService = inventoryService;
        }

        private async Task<User> GetUser(int userId)
        {
            User? user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                Exception exception = new();
                exception.Data["ErrorType"] = ErrorType.NotFoundUser;
                throw exception;
            }
            return user;
        }

        public async Task<int> ChangeCurrencyAsync(int userId, int modifier)
        {
            User user = await GetUser(userId);


            user.AvailableCurrency += modifier;
            return user.AvailableCurrency;
        }

        // TODO: implement email change verification by service
        public async Task<bool> ChangeEmailAsync(int userId, string newEmail)
        {
            User user = await GetUser(userId);



            return true;
        }

        // TODO: implement 
        public Task<bool> ChangePasswordAsync(int userId, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ChangeUsernameAsync(int userId, string newUsername)
        {
            throw new NotImplementedException();
        }

        public async Task<User> CreateUserAsync(UserRegisterDTO model)
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
            // TODO: add default access features, avatar, UserState, Settings
            return await userRepository.AddAsync(user);
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserDTO>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Manage2FAAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUserAsync(int userId, UserDTO userUpdateDTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Verify2FAAsync(int userId, string token)
        {
            throw new NotImplementedException();
        }
    }
}

