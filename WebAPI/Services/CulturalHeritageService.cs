using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("search")]
        public async Task<IEnumerable<CulturalHeritageDto>> SearchCulturalHeritages(string name)
        {
            return await _dbContext.CulturalHeritages
                .Where(ch => ch.Name.Contains(name))
                .Include(ch => ch.CulturalHeritageThemes) // Navigate through the join table
                    .ThenInclude(ct => ct.Theme)
                .Select(ch => new CulturalHeritageDto
                {
                    HeritageID = ch.HeritageId,
                    Name = ch.Name,
                    Description = ch.Description,
                    Location = ch.Location,
                    NationalMinority = ch.NationalMinority,
                    Themes = ch.CulturalHeritageThemes.Select(ct => new ThemeDto
                    {
                        ThemeID = ct.Theme.ThemeID,
                        Name = ct.Theme.Name,
                        Description = ct.Theme.Description
                    }).ToList()
                })
                .ToListAsync();
        }
        public async Task<bool> IsNameUniqueAsync(string name, int? id = null)
        {
            return !await _dbContext.CulturalHeritages
                .AnyAsync(ch => ch.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                                && (!id.HasValue || ch.HeritageId != id));
        }

        [HttpGet]
        public async Task<IEnumerable<CulturalHeritageDto>> GetAllCulturalHeritages()
        {
            return await _dbContext.CulturalHeritages
                .Include(ch => ch.CulturalHeritageThemes) // Include join table
                    .ThenInclude(ct => ct.Theme)        // Include related themes
                .Select(ch => new CulturalHeritageDto
                {
                    HeritageID = ch.HeritageId,
                    Name = ch.Name,
                    Description = ch.Description,
                    Location = ch.Location,
                    NationalMinority = ch.NationalMinority,
                    Themes = ch.CulturalHeritageThemes.Select(ct => new ThemeDto
                    {
                        ThemeID = ct.Theme.ThemeID, // Match with updated Theme class
                        Name = ct.Theme.Name,
                        Description = ct.Theme.Description
                    }).ToList()
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<CulturalHeritageDto?> GetCulturalHeritageById(int heritageId)
        {
            var heritage = await _dbContext.CulturalHeritages
                .Include(ch => ch.CulturalHeritageThemes) // Navigate through the join table
                    .ThenInclude(ct => ct.Theme)
                .FirstOrDefaultAsync(ch => ch.HeritageId == heritageId);

            if (heritage == null) return null;

            return new CulturalHeritageDto
            {
                HeritageID = heritage.HeritageId,
                Name = heritage.Name,
                Description = heritage.Description,
                Location = heritage.Location,
                NationalMinority = heritage.NationalMinority,
                Themes = heritage.CulturalHeritageThemes.Select(ct => new ThemeDto
                {
                    ThemeID = ct.Theme.ThemeID,
                    Name = ct.Theme.Name,
                    Description = ct.Theme.Description
                }).ToList()
            };
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
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
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
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
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
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
