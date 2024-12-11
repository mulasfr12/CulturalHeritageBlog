using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Services.Interfaces;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CulturalHeritageController : Controller
    {
        private readonly ICulturalHeritageService _culturalHeritageService;
        private readonly IMapper _mapper;
        public CulturalHeritageController(ICulturalHeritageService culturalHeritageService, IMapper mapper)
        {
            _culturalHeritageService = culturalHeritageService;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(string search, int? nationalMinority, int page = 1, int pageSize = 10)
        {
            var heritages = await _culturalHeritageService.GetAllCulturalHeritages();

            // Filter by search term
            if (!string.IsNullOrEmpty(search))
            {
                heritages = heritages.Where(h => h.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Filter by national minority (dropdown)
            if (nationalMinority.HasValue)
            {
                heritages = heritages.Where(h => h.NationalMinority == nationalMinority.ToString()).ToList();
            }

            // Paging
            var totalItems = heritages.Count();
            var paginatedHeritages = heritages.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new CulturalHeritageListViewModel
            {
                Heritages = paginatedHeritages,
                Search = search,
                NationalMinority = nationalMinority,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var heritageDto = await _culturalHeritageService.GetCulturalHeritageById(id);
            if (heritageDto == null)
            {
                return NotFound();
            }

            // Use AutoMapper to map DTO to ViewModel
            var viewModel = _mapper.Map<CulturalHeritageViewModel>(heritageDto);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CulturalHeritageViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var heritageDto = _mapper.Map<CulturalHeritageDto>(viewModel);
                await _culturalHeritageService.UpdateCulturalHeritage(viewModel.HeritageID, heritageDto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Check for WebAPI validation errors
                ViewBag.ErrorMessage = ex.Message.Contains("already exists")
                    ? "A cultural heritage with this name already exists."
                    : "An error occurred while updating the cultural heritage.";
                return View(viewModel);
            }
        }

        public IActionResult Create()
        {
            return View(new CulturalHeritageViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CulturalHeritageViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var heritageDto = _mapper.Map<CulturalHeritageDto>(viewModel);
                await _culturalHeritageService.CreateCulturalHeritage(heritageDto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Check for WebAPI validation errors
                ViewBag.ErrorMessage = ex.Message.Contains("already exists")
                    ? "A cultural heritage with this name already exists."
                    : "An error occurred while creating the cultural heritage.";
                return View(viewModel);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var heritage = await _culturalHeritageService.GetCulturalHeritageById(id);
            if (heritage == null)
            {
                return NotFound();
            }
            return View(heritage);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            await _culturalHeritageService.DeleteCulturalHeritage(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

