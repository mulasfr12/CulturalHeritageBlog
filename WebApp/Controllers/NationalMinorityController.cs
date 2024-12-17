using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Services.Interfaces;

namespace CulturalHeritageWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class NationalMinoritiesController : Controller
    {
        private readonly INationalMinorityService _nationalMinorityService;

        public NationalMinoritiesController(INationalMinorityService nationalMinorityService)
        {
            _nationalMinorityService = nationalMinorityService;
        }

        public async Task<IActionResult> Index()
        {
            var minorities = await _nationalMinorityService.GetAllNationalMinorities();
            return View(minorities);
        }

        public IActionResult Create()
        {
            return View(new NationalMinorityDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(NationalMinorityDto minorityDto)
        {
            if (!ModelState.IsValid)
            {
                return View(minorityDto);
            }

            await _nationalMinorityService.CreateNationalMinority(minorityDto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var minority = await _nationalMinorityService.GetNationalMinorityById(id);
            if (minority == null)
            {
                return NotFound();
            }

            return View(minority);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(NationalMinorityDto minorityDto)
        {
            if (!ModelState.IsValid)
            {
                return View(minorityDto);
            }

            await _nationalMinorityService.UpdateNationalMinority(minorityDto.MinorityID, minorityDto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var minority = await _nationalMinorityService.GetNationalMinorityById(id);
            if (minority == null)
            {
                return NotFound();
            }

            return View(minority);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            try
            {
                var result = await _nationalMinorityService.DeleteNationalMinority(id);
                if (!result)
                {
                    TempData["ErrorMessage"] = "Failed to delete. National Minority not found.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "National Minority deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
