using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Services.Interfaces;
using WebApp.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using WebAPI.DTOs;

namespace WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService; // Reference to WebAPI service

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please provide valid username and password.";
                return View();
            }
            var loginDto = new UserLoginDto
            {
                Username = model.Username,
                Password = model.Password
            };

            var token = await _userService.LoginUser(loginDto);
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.ErrorMessage = "Invalid login credentials.";
                return View();
            }

            // Decode the token to extract role
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var claims = jwtToken.Claims.ToList();
            var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Create identity and sign in
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Redirect based on role
            if (role == "Admin")
                return RedirectToAction("AdminDashboard", "Dashboard");
            else
                return RedirectToAction("UserDashboard", "Dashboard");
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please provide all required information.";
                return View();
            }

            if (model.Password != model.ConfirmPassword)
            {
                ViewBag.ErrorMessage = "Passwords do not match.";
                return View();
            }

            var userDto = new UserRegisterDto
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password
            };

            var result = await _userService.RegisterUser(userDto);

            if (!result)
            {
                ViewBag.ErrorMessage = "Registration failed. Username or email might already be in use.";
                return View();
            }

            // Redirect to login after successful registration
            return RedirectToAction("Login");
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home"); // Redirect to the landing page
        }
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
    }
}
