using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Services.Interfaces;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserProfileController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);
            var profile = await _userService.GetUserById(userId);

            if (profile == null)
                return NotFound(new { message = "User profile not found." });

            return Ok(profile);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UpdateUserProfileDto profileDto)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);

            var result = await _userService.UpdateUserProfile(userId, profileDto);

            if (!result)
                return BadRequest(new { message = "Update failed. Please check the provided information." });

            return NoContent();
        }
    }
}
