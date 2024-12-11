using WebAPI.DTOs;

namespace WebAPI.Services.Interfaces
{
    public interface IThemeService
    {
        Task<IEnumerable<ThemeDto>> GetAllThemes();
        Task<ThemeDto> GetThemeById(int themeId);
        Task<int> CreateTheme(ThemeDto themeDto);
        Task<bool> UpdateTheme(int themeId, ThemeDto themeDto);
        Task<bool> DeleteTheme(int themeId);
        Task<bool> AddThemeToCulturalHeritage(int heritageId, int themeId, string description);
        Task<bool> RemoveThemeFromCulturalHeritage(int heritageId, int themeId);
        Task<IEnumerable<ThemeDto>> GetThemesByCulturalHeritage(int heritageId);

    }
}
