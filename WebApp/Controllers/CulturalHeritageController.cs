using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Services;
using WebAPI.Services.Interfaces;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CulturalHeritageController : Controller
    {
        private readonly ICulturalHeritageService _culturalHeritageService;
        private readonly IMapper _mapper;
        private readonly INationalMinorityService _nationalMinorityService;
        private readonly  IThemeService _themeService;
        private readonly ICommentService _commentService;

        public CulturalHeritageController(ICulturalHeritageService culturalHeritageService, ICommentService commentService, IMapper mapper, INationalMinorityService 
            nationalMinorityService,IThemeService themeService)
        {
            _nationalMinorityService = nationalMinorityService;
            _culturalHeritageService = culturalHeritageService;
            _commentService = commentService;
            _mapper = mapper;
            _themeService = themeService;
        }
        public async Task<IActionResult> Index(string search, int? nationalMinority, int page = 1, int pageSize = 10)
        {
            var heritages = await _culturalHeritageService.GetAllCulturalHeritages();

            // Filter by search term
            if (!string.IsNullOrEmpty(search))
            {
                heritages = heritages
                    .Where(h => h.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filter by National Minority ID
            if (nationalMinority.HasValue)
            {
                heritages = heritages
                    .Where(h => h.NationalMinorityID == nationalMinority.Value)
                    .ToList();
            }

            // Paging
            var totalItems = heritages.Count();
            var paginatedHeritages = heritages
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Fetch National Minorities
            var minorities = await _nationalMinorityService.GetAllNationalMinorities();

            var viewModel = new CulturalHeritageListViewModel
            {
                Heritages = paginatedHeritages,
                Search = search,
                NationalMinority = nationalMinority,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                NationalMinorities = minorities
            };

            return View(viewModel);
        }
        public async Task<IActionResult> Edit(int id)
        {
            // Fetch Cultural Heritage data by ID
            var heritageDto = await _culturalHeritageService.GetCulturalHeritageById(id);
            if (heritageDto == null)
            {
                return NotFound();
            }

            // Map DTO to ViewModel
            var viewModel = _mapper.Map<CulturalHeritageViewModel>(heritageDto);

            // Fetch National Minorities for the dropdown
            var minorities = await _nationalMinorityService.GetAllNationalMinorities();
            ViewBag.NationalMinorities = _mapper.Map<List<NationalMinorityViewModel>>(minorities);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CulturalHeritageViewModel viewModel)
        {
            Console.WriteLine($"Received HeritageID: {viewModel.HeritageID}");

            if (!ModelState.IsValid)
            {
                var minorities = await _nationalMinorityService.GetAllNationalMinorities();
                ViewBag.NationalMinorities = _mapper.Map<List<NationalMinorityViewModel>>(minorities);
                return View(viewModel);
            }

            try
            {
                var heritageDto = _mapper.Map<CulturalHeritageDto>(viewModel);
                Console.WriteLine($"Updating Cultural Heritage with ID: {heritageDto.HeritageID}, Name: {heritageDto.Name}, MinorityID: {heritageDto.NationalMinorityID}");

                var updated = await _culturalHeritageService.UpdateCulturalHeritage(viewModel.HeritageID, heritageDto);

                if (!updated)
                {
                    ViewBag.ErrorMessage = "Error updating the Cultural Heritage. Please try again.";
                    var minorities = await _nationalMinorityService.GetAllNationalMinorities();
                    ViewBag.NationalMinorities = _mapper.Map<List<NationalMinorityViewModel>>(minorities);
                    return View(viewModel);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Unexpected error: {ex.Message}";
                var minorities = await _nationalMinorityService.GetAllNationalMinorities();
                ViewBag.NationalMinorities = _mapper.Map<List<NationalMinorityViewModel>>(minorities);
                return View(viewModel);
            }
        }


        public async Task<IActionResult> Create()
        {
            // Initialize an empty model
            var viewModel = new CulturalHeritageViewModel();

            // Fetch National Minorities for the dropdown
            var minorities = await _nationalMinorityService.GetAllNationalMinorities();
            ViewBag.NationalMinorities = _mapper.Map<List<NationalMinorityViewModel>>(minorities);

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CulturalHeritageViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate National Minorities in case of validation errors
                var minorities = await _nationalMinorityService.GetAllNationalMinorities();
                ViewBag.NationalMinorities = _mapper.Map<List<NationalMinorityViewModel>>(minorities);

                return View(viewModel);
            }

            try
            {
                // Map ViewModel to DTO
                var heritageDto = _mapper.Map<CulturalHeritageDto>(viewModel);

                // Create the Cultural Heritage
                await _culturalHeritageService.CreateCulturalHeritage(heritageDto);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;

                // Repopulate National Minorities in case of errors
                var minorities = await _nationalMinorityService.GetAllNationalMinorities();
                ViewBag.NationalMinorities = _mapper.Map<List<NationalMinorityViewModel>>(minorities);

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
        [AllowAnonymous] // Accessible to Users without admin roles    
        public async Task<IActionResult> UserIndex()
        {
            // Fetch cultural heritages
            var heritages = await _culturalHeritageService.GetAllCulturalHeritages();

            // Map to the list of CulturalHeritageViewModel
            var viewModels = _mapper.Map<List<CulturalHeritageViewModel>>(heritages);

            // Return the UserIndex view with the mapped models
            return View(viewModels);
        }


        [AllowAnonymous] // Accessible to Users without admin roles
        public async Task<IActionResult> Details(int id)
        {
            // Fetch cultural heritage details
            var heritageDto = await _culturalHeritageService.GetCulturalHeritageById(id);
            if (heritageDto == null)
            {
                return NotFound();
            }

            // Fetch related themes
            var themes = await _themeService.GetThemesByCulturalHeritage(id);
            var mappedThemes = _mapper.Map<List<ThemeViewModel>>(themes);

            // Fetch related comments
            var comments = await _commentService.GetCommentsByCulturalHeritageId(id);
            var mappedComments = _mapper.Map<List<CommentViewModel>>(comments);

            // Map to the details view model
            var viewModel = _mapper.Map<CulturalHeritageDetailsViewModel>(heritageDto);
            viewModel.Themes = mappedThemes;
            viewModel.Comments = mappedComments;

            // Pass the ViewModel to the view
            return View(viewModel);
        }


    }
}

