using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Services.Interfaces;

namespace WebAPI.Services
{
    public class ThemeService : IThemeService
    {
        private readonly CulturalHeritageDbContext _dbContext;

        public ThemeService(CulturalHeritageDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<ThemeDto>> GetThemesByCulturalHeritage(int heritageId)
        {
            return await _dbContext.CulturalHeritageThemes
                .Where(ct => ct.HeritageId == heritageId)
                .Select(ct => new ThemeDto
                {
                    ThemeID = ct.ThemeId,
                    Name = ct.Theme.Name,
                    Description = ct.Description
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ThemeDto>> GetAllThemes()
        {
            return await _dbContext.Themes
                .Select(t => new ThemeDto
                {
                    ThemeID = t.ThemeId,
                    Name = t.Name,
                    Description = t.Description
                })
                .ToListAsync();
        }

        public async Task<ThemeDto> GetThemeById(int themeId)
        {
            var theme = await _dbContext.Themes.FindAsync(themeId);
            if (theme == null) return null;

            return new ThemeDto
            {
                ThemeID = theme.ThemeId,
                Name = theme.Name,
                Description = theme.Description
            };
        }

        public async Task<int> CreateTheme(ThemeDto themeDto)
        {
            var theme = new Theme
            {
                Name = themeDto.Name,
                Description = themeDto.Description
            };

            _dbContext.Themes.Add(theme);
            await _dbContext.SaveChangesAsync();

            return theme.ThemeId;
        }

        public async Task<bool> UpdateTheme(int themeId, ThemeDto themeDto)
        {
            var theme = await _dbContext.Themes.FindAsync(themeId);
            if (theme == null) return false;

            theme.Name = themeDto.Name;
            theme.Description = themeDto.Description;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTheme(int themeId)
        {
            var theme = await _dbContext.Themes.FindAsync(themeId);
            if (theme == null) return false;

            _dbContext.Themes.Remove(theme);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddThemeToCulturalHeritage(int heritageId, int themeId, string description)
        {
            var heritage = await _dbContext.CulturalHeritages.FindAsync(heritageId);
            var theme = await _dbContext.Themes.FindAsync(themeId);

            if (heritage == null || theme == null) return false;

            var culturalHeritageTheme = new CulturalHeritageTheme
            {
                HeritageId = heritageId,
                ThemeId = themeId,
                Description = description
            };

            _dbContext.CulturalHeritageThemes.Add(culturalHeritageTheme);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveThemeFromCulturalHeritage(int heritageId, int themeId)
        {
            var culturalHeritageTheme = await _dbContext.CulturalHeritageThemes
                .FirstOrDefaultAsync(ct => ct.HeritageId == heritageId && ct.ThemeId == themeId);

            if (culturalHeritageTheme == null) return false;

            _dbContext.CulturalHeritageThemes.Remove(culturalHeritageTheme);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
