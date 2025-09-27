using Application.DTOs;

namespace Application.Abstractions.Interfaces.Service.UserCore
{
    public interface IAuthService
    {
        Task<UserDTO> Register(UserRegisterDTO userRegisterDTO);
        // Task<string> Login(string , string password);
    }
}
