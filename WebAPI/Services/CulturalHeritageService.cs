using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Services.Interfaces;

namespace WebAPI.Services
{
    public class CulturalHeritageService : ICulturalHeritageService
    {
        private readonly CulturalHeritageDbContext _dbContext;

        public CulturalHeritageService(CulturalHeritageDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<CulturalHeritageDto>> GetAllCulturalHeritages()
        {
            return await _dbContext.CulturalHeritages
                .Include(ch => ch.Themes) // Include themes
                .Select(ch => new CulturalHeritageDto
                {
                    HeritageID = ch.HeritageId,
                    Name = ch.Name,
                    Description = ch.Description,
                    Location = ch.Location,
                    NationalMinority = ch.NationalMinority,
                    Themes = ch.Themes.Select(t => new ThemeDto
                    {
                        ThemeID = t.ThemeId,
                        Name = t.Name,
                        Description = t.Description
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<CulturalHeritageDto> GetCulturalHeritageById(int heritageId)
        {
            var heritage = await _dbContext.CulturalHeritages
                .Include(ch => ch.Themes)
                .FirstOrDefaultAsync(ch => ch.HeritageId == heritageId);

            if (heritage == null) return null;

            return new CulturalHeritageDto
            {
                HeritageID = heritage.HeritageId,
                Name = heritage.Name,
                Description = heritage.Description,
                Location = heritage.Location,
                NationalMinority = heritage.NationalMinority,
                Themes = heritage.Themes.Select(t => new ThemeDto
                {
                    ThemeID = t.ThemeId,
                    Name = t.Name,
                    Description = t.Description
                }).ToList()
            };
        }

        public async Task<int> CreateCulturalHeritage(CulturalHeritageDto heritageDto)
        {
            var heritage = new CulturalHeritage
            {
                Name = heritageDto.Name,
                Description = heritageDto.Description,
                Location = heritageDto.Location,
                NationalMinority = heritageDto.NationalMinority,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.CulturalHeritages.Add(heritage);
            await _dbContext.SaveChangesAsync();

            return heritage.HeritageId;
        }

        public async Task<bool> UpdateCulturalHeritage(int heritageId, CulturalHeritageDto heritageDto)
        {
            var heritage = await _dbContext.CulturalHeritages.FindAsync(heritageId);

            if (heritage == null) return false;

            heritage.Name = heritageDto.Name;
            heritage.Description = heritageDto.Description;
            heritage.Location = heritageDto.Location;
            heritage.NationalMinority = heritageDto.NationalMinority;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCulturalHeritage(int heritageId)
        {
            var heritage = await _dbContext.CulturalHeritages.FindAsync(heritageId);

            if (heritage == null) return false;

            _dbContext.CulturalHeritages.Remove(heritage);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
