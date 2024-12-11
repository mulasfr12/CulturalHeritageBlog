using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Services.Interfaces;

[Authorize(Roles = "Admin")]
public class CulturalHeritageThemesController : Controller
{
    private readonly IThemeService _themeService;

    public CulturalHeritageThemesController(IThemeService themeService)
    {
        _themeService = themeService;
    }

    public async Task<IActionResult> Index(int heritageId)
    {
        var themes = await _themeService.GetThemesByCulturalHeritage(heritageId);
        ViewBag.HeritageId = heritageId;
        return View(themes);
    }

    public IActionResult Add(int heritageId)
    {
        ViewBag.HeritageId = heritageId;
        return View(new ThemeDto());
    }

    [HttpPost]
    public async Task<IActionResult> Add(int heritageId, int themeId, string description)
    {
        await _themeService.AddThemeToCulturalHeritage(heritageId, themeId, description);
        return RedirectToAction(nameof(Index), new { heritageId });
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int heritageId, int themeId)
    {
        await _themeService.RemoveThemeFromCulturalHeritage(heritageId, themeId);
        return RedirectToAction(nameof(Index), new { heritageId });
    }
}
