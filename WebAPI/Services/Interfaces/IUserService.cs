using WebAPI.DTOs;
using WebAPI.Models;

namespace WebAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterUser(UserRegisterDto registerDto);
        Task<string> LoginUser(UserLoginDto loginDto);
        Task<bool> ChangePassword(string username, string newPassword);
        Task<User> GetUserById(int userId);
        Task<bool> UpdateUserProfile(int userId, UpdateUserProfileDto profileDto);
    }
}
