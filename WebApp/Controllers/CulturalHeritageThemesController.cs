using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Services.Interfaces;
using WebApp.ViewModels;

[Authorize(Roles = "Admin")]
public class CulturalHeritageThemesController : Controller
{
    private readonly IThemeService _themeService;
    private readonly ICulturalHeritageService _culturalHeritageService;
    private readonly IMapper _mapper;

    public CulturalHeritageThemesController(IThemeService themeService,ICulturalHeritageService culturalHeritageService,IMapper mapper)
    {
        _themeService = themeService;
        _culturalHeritageService = culturalHeritageService;
        _mapper = mapper;

    }

    public async Task<IActionResult> Index(int heritageId)
    {
        var themes = await _themeService.GetThemesByCulturalHeritage(heritageId);
        ViewBag.HeritageId = heritageId;
        return View(themes);
    }

    public async Task<IActionResult> Add()
    {
        // Fetch all cultural heritages for dropdown
        var culturalHeritages = await _culturalHeritageService.GetAllCulturalHeritages();
        ViewBag.CulturalHeritages = _mapper.Map<List<CulturalHeritageViewModel>>(culturalHeritages);

        // Fetch all available themes
        var allThemes = await _themeService.GetAllThemes();
        ViewBag.AllThemes = allThemes;

        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Add(int heritageId, int themeId, string description)
    {
        if (heritageId == 0 || themeId == 0)
        {
            ViewBag.ErrorMessage = "Please select both a Cultural Heritage and a Theme.";

            // Fetch dropdown data again in case of error
            ViewBag.CulturalHeritages = _mapper.Map<List<CulturalHeritageViewModel>>(await _culturalHeritageService.GetAllCulturalHeritages());
            ViewBag.AllThemes = await _themeService.GetAllThemes();

            return View();
        }

        try
        {
            // Associate the theme with the cultural heritage
            await _themeService.AddThemeToCulturalHeritage(heritageId, themeId, description);
            return RedirectToAction(nameof(Index), new { heritageId });
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"Error: {ex.Message}";

            // Fetch dropdown data again in case of error
            ViewBag.CulturalHeritages = _mapper.Map<List<CulturalHeritageViewModel>>(await _culturalHeritageService.GetAllCulturalHeritages());
            ViewBag.AllThemes = await _themeService.GetAllThemes();

            return View();
        }
    }
    public async Task<IActionResult> Edit(int heritageId)
    {
        var heritage = await _culturalHeritageService.GetCulturalHeritageById(heritageId);
        if (heritage == null)
        {
            return NotFound();
        }

        ViewBag.CulturalHeritages = await _culturalHeritageService.GetAllCulturalHeritages();
        ViewBag.CurrentHeritage = heritage.Name;

        var themes = await _themeService.GetThemesByCulturalHeritage(heritageId);
        return View(themes);
    }

    [HttpPost]
    public async Task<IActionResult> RemoveTheme(int heritageId, int themeId)
    {
        await _themeService.RemoveThemeFromCulturalHeritage(heritageId, themeId);
        return RedirectToAction(nameof(Edit), new { heritageId });
    }
}
