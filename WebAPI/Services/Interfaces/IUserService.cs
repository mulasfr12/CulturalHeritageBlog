using WebAPI.DTOs;

namespace WebAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterUser(UserRegisterDto registerDto);
        Task<string> LoginUser(UserLoginDto loginDto);
        Task<bool> ChangePassword(string username, string newPassword);
    }
}
