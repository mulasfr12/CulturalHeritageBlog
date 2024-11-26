using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Services.Interfaces;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints are secured by default
    public class ThemeController : ControllerBase
    {
        private readonly IThemeService _themeService;
        private readonly ILogServices _logServices;

        public ThemeController(IThemeService themeService, ILogServices logServices)
        {
            _themeService = themeService;
            _logServices = logServices;
        }

        // GET: api/theme
        [HttpGet]
        [AllowAnonymous] // Allow public access
        public async Task<IActionResult> GetAllThemes()
        {
            var themes = await _themeService.GetAllThemes();
            return Ok(themes);
        }

        // GET: api/theme/{id}
        [HttpGet("{id}")]
        [AllowAnonymous] // Allow public access
        public async Task<IActionResult> GetThemeById(int id)
        {
            var theme = await _themeService.GetThemeById(id);
            if (theme == null) return NotFound(new { message = $"Theme with id={id} not found." });
            return Ok(theme);
        }

        // POST: api/theme
        [HttpPost]
        [Authorize(Roles = "Admin")] // Admin-only
        public async Task<IActionResult> CreateTheme(ThemeDto themeDto)
        {
            try
            {
                var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);
                var themeId = await _themeService.CreateTheme(themeDto);

                await _logServices.AddLog($"Theme with id={themeId} has been created.", userId);

                return CreatedAtAction(nameof(GetThemeById), new { id = themeId }, themeDto);
            }
            catch (Exception ex)
            {
                var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);
                await _logServices.AddLog($"Error creating theme: {ex.Message}", userId);
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        // PUT: api/theme/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Admin-only
        public async Task<IActionResult> UpdateTheme(int id, ThemeDto themeDto)
        {
            try
            {
                var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);
                var result = await _themeService.UpdateTheme(id, themeDto);

                if (!result)
                {
                    await _logServices.AddLog($"Theme with id={id} not found for update.", userId);
                    return NotFound(new { message = "Theme not found." });
                }

                await _logServices.AddLog($"Theme with id={id} has been updated.", userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);
                await _logServices.AddLog($"Error updating theme with id={id}: {ex.Message}", userId);
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        // DELETE: api/theme/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Admin-only
        public async Task<IActionResult> DeleteTheme(int id)
        {
            try
            {
                var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);
                var result = await _themeService.DeleteTheme(id);

                if (!result)
                {
                    await _logServices.AddLog($"Theme with id={id} not found for deletion.", userId);
                    return NotFound(new { message = "Theme not found." });
                }

                await _logServices.AddLog($"Theme with id={id} has been deleted.", userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);
                await _logServices.AddLog($"Error deleting theme with id={id}: {ex.Message}", userId);
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        // POST: api/theme/{heritageId}/add/{themeId}
        [HttpPost("{heritageId}/add/{themeId}")]
        [Authorize(Roles = "Admin")] // Admin-only
        public async Task<IActionResult> AddThemeToCulturalHeritage(int heritageId, int themeId, [FromBody] string description)
        {
            try
            {
                var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);
                var result = await _themeService.AddThemeToCulturalHeritage(heritageId, themeId, description);

                if (!result)
                {
                    return BadRequest(new { message = "Either Cultural Heritage or Theme not found." });
                }

                await _logServices.AddLog($"Theme with id={themeId} added to Cultural Heritage with id={heritageId}.", userId);
                return Ok(new { message = "Theme successfully added to Cultural Heritage." });
            }
            catch (Exception ex)
            {
                var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);
                await _logServices.AddLog($"Error adding theme to cultural heritage: {ex.Message}", userId);
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        // DELETE: api/theme/{heritageId}/remove/{themeId}
        [HttpDelete("{heritageId}/remove/{themeId}")]
        [Authorize(Roles = "Admin")] // Admin-only
        public async Task<IActionResult> RemoveThemeFromCulturalHeritage(int heritageId, int themeId)
        {
            try
            {
                var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);
                var result = await _themeService.RemoveThemeFromCulturalHeritage(heritageId, themeId);

                if (!result)
                {
                    return NotFound(new { message = "The association between Cultural Heritage and Theme was not found." });
                }

                await _logServices.AddLog($"Theme with id={themeId} removed from Cultural Heritage with id={heritageId}.", userId);
                return Ok(new { message = "Theme successfully removed from Cultural Heritage." });
            }
            catch (Exception ex)
            {
                var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);
                await _logServices.AddLog($"Error removing theme from cultural heritage: {ex.Message}", userId);
                return StatusCode(500, new { message = "Internal server error." });
            }
        }
    }
}
