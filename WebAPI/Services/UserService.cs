using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Services.Interfaces;
using BCrypt.Net;


namespace WebAPI.Services
{
    public class UserService : IUserService
    {
        private readonly CulturalHeritageDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public UserService(CulturalHeritageDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<bool> RegisterUser(UserRegisterDto registerDto)
        {
            // Check if the username or email already exists
            if (await _dbContext.Users.AnyAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email))
            {
                return false; // User already exists
            }

            // Hash the password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Determine the role (default to "User" if none is provided)
            var role = string.IsNullOrEmpty(registerDto.Role) ? "User" : registerDto.Role;

            // Create and add the user to the database
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = hashedPassword,
                Role = role, // Assign the role
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }


        public async Task<string> LoginUser(UserLoginDto loginDto)
        {
            // Check if the user exists
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null; // Invalid username or password
            }

            // Generate JWT token
            return GenerateJwtToken(user);
        }

        public async Task<bool> ChangePassword(string username, string newPassword)
        {
            // Find the user
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return false; // User not found
            }

            // Hash the new password and update it
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role), // Include the user's role
        new Claim("UserID", user.UserId.ToString()) // Add UserID claim
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<User> GetUserById(int userId)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<bool> UpdateUserProfile(int userId, UpdateUserProfileDto profileDto)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null || !VerifyPassword(user.PasswordHash, profileDto.CurrentPassword))
                return false;

            user.Username = profileDto.Username ?? user.Username;
            user.Email = profileDto.Email ?? user.Email;

            if (!string.IsNullOrWhiteSpace(profileDto.NewPassword))
            {
                user.PasswordHash = HashPassword(profileDto.NewPassword);
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        private string HashPassword(string password)
        {
            // Implement password hashing logic
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string hashedPassword, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
