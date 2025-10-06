using Application.DTOs;

namespace Application.Abstractions.Interfaces.Services.UserCore
{
    public interface IAuthService
    {
        Task<UserDTO> Register(UserRegisterDTO userRegisterDTO);
        // Task<string> Login(string , string password);
    }
}
