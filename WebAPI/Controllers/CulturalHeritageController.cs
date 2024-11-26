using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Services;
using WebAPI.Services.Interfaces;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CulturalHeritageController : Controller
    {
        private readonly ICulturalHeritageService _culturalHeritageService;
        private readonly ILogServices _logServices;

        public CulturalHeritageController(ICulturalHeritageService culturalHeritageService,ILogServices logServices)
        {
            _culturalHeritageService = culturalHeritageService;
            _logServices = logServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCulturalHeritages()
        {
            var heritages = await _culturalHeritageService.GetAllCulturalHeritages();
            return Ok(heritages);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCulturalHeritageById(int id)
        {
            var heritage = await _culturalHeritageService.GetCulturalHeritageById(id);
            if (heritage == null) return NotFound(new { message = "Cultural Heritage not found" });
            return Ok(heritage);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> CreateCulturalHeritage(CulturalHeritageDto culturalHeritageDto)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                {
                    return Unauthorized(new { message = "Authentication is required to perform this action." });
                }

                var userClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID");
                if (userClaim == null)
                {
                    return Unauthorized(new { message = "UserID claim is missing in the token." });
                }

                var userId = int.Parse(userClaim.Value);

                var heritageId = await _culturalHeritageService.CreateCulturalHeritage(culturalHeritageDto);

                await _logServices.AddLog($"Cultural heritage with id={heritageId} has been created.", userId);

                return CreatedAtAction(nameof(GetCulturalHeritageById), new { id = heritageId }, culturalHeritageDto);
            }
            catch (Exception ex)
            {
                // Log the error
                var userId = 0; // Default value in case User.Claims is not available
                if (User != null && User.Claims.Any(c => c.Type == "UserID"))
                {
                    userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);
                }

                await _logServices.AddLog($"Error while creating cultural heritage: {ex.Message}", userId);

                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCulturalHeritage(int id, CulturalHeritageDto culturalHeritageDto)
        {
            try
            {
                var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);
                var result = await _culturalHeritageService.UpdateCulturalHeritage(id, culturalHeritageDto);

                if (!result)
                {
                    await _logServices.AddLog($"Cultural heritage with id={id} not found for update.", userId);
                    return NotFound(new { message = "Cultural heritage not found" });
                }

                await _logServices.AddLog($"Cultural heritage with id={id} has been updated.", userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);
                await _logServices.AddLog($"Error while updating cultural heritage with id={id}: {ex.Message}", userId);

                return StatusCode(500, "Internal server error");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCulturalHeritage(int id)
        {
            try
            {
                var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);
                var result = await _culturalHeritageService.DeleteCulturalHeritage(id);

                if (!result)
                {
                    await _logServices.AddLog($"Cultural heritage with id={id} not found for deletion.", userId);
                    return NotFound(new { message = "Cultural heritage not found" });
                }

                await _logServices.AddLog($"Cultural heritage with id={id} has been deleted.", userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                var userId = int.Parse(User.Claims.First(c => c.Type == "UserID").Value);
                await _logServices.AddLog($"Error while deleting cultural heritage with id={id}: {ex.Message}", userId);

                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchCulturalHeritages([FromQuery] string name)
        {
            var heritages = await _culturalHeritageService.SearchCulturalHeritages(name);
            return Ok(heritages);
        }
    }
}
