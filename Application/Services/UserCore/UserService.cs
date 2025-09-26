using Application.Abstractions.Interfaces.Repository.User;
using Application.Abstractions.Interfaces.Services;
using Application.DTOs;
using Entities.Enums;
using Entities.Models.UserCore;

namespace Application.Services.UserCore
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
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

        public Task<UserDTO> CreateUserAsync(UserRegisterDTO userRegisterDTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserDTO>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Entities.Models.UserCore.User> GetByIdAsync(int id)
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

