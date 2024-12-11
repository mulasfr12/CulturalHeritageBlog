using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Services.Interfaces;

namespace CulturalHeritageWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ThemesController : Controller
    {
        private readonly IThemeService _themeService;

        public ThemesController(IThemeService themeService)
        {
            _themeService = themeService;
        }

        public async Task<IActionResult> Index()
        {
            var themes = await _themeService.GetAllThemes();
            return View(themes);
        }

        public IActionResult Create()
        {
            return View(new ThemeDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ThemeDto themeDto)
        {
            if (!ModelState.IsValid)
            {
                return View(themeDto);
            }

            await _themeService.CreateTheme(themeDto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var theme = await _themeService.GetThemeById(id);
            if (theme == null)
            {
                return NotFound();
            }

            return View(theme);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ThemeDto themeDto)
        {
            if (!ModelState.IsValid)
            {
                return View(themeDto);
            }

            await _themeService.UpdateTheme(themeDto.ThemeID, themeDto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var theme = await _themeService.GetThemeById(id);
            if (theme == null)
            {
                return NotFound();
            }

            return View(theme);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            await _themeService.DeleteTheme(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
