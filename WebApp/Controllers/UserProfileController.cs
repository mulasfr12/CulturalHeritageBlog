using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Services.Interfaces;
using WebApp.ViewModels;

[Authorize]
public class UserProfileController : Controller
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UserProfileController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        // Get the current user's details
        var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);
        var user = await _userService.GetUserById(userId);

        if (user == null)
        {
            return NotFound();
        }

        // Map to UserProfileViewModel
        var viewModel = _mapper.Map<UserProfileViewModel>(user);

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Update(UserProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);

        // Map to UpdateUserProfileDto
        var updateDto = _mapper.Map<UpdateUserProfileDto>(model);

        // Call the API to update the user profile
        var result = await _userService.UpdateUserProfile(userId, updateDto);

        if (!result)
        {
            ViewBag.ErrorMessage = "Failed to update profile. Please try again.";
            return View("Index", model);
        }

        // Check user role
        var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        TempData["SuccessMessage"] = "Profile updated successfully.";

        // Redirect based on role
        if (userRole == "Admin")
        {
            return RedirectToAction("AdminDashboard", "Dashboard");
        }
        else
        {
            return RedirectToAction("UserDashboard", "Dashboard");
        }
    }

}
