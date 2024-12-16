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
                .Include(ch => ch.NationalMinority) // Include National Minority for display
                .Select(ch => new CulturalHeritageDto
                {
                    HeritageID = ch.HeritageId,
                    Name = ch.Name,
                    Description = ch.Description,
                    Location = ch.Location,
                    NationalMinorityID = ch.NationalMinorityId,
                    NationalMinorityName = ch.NationalMinority != null ? ch.NationalMinority.Name : null, // Avoid ?. operator
                    Themes = ch.CulturalHeritageThemes.Select(ct => new ThemeDto
                    {
                        ThemeID = ct.Theme.ThemeId,
                        Name = ct.Theme.Name,
                        Description = ct.Theme.Description
                    }).ToList()
                })
                .ToListAsync();
        }


        public async Task<bool> IsNameUniqueAsync(string name, int? id = null)
        {
            return !await _dbContext.CulturalHeritages
                .AnyAsync(ch => ch.Name.ToLower() == name.ToLower() && (!id.HasValue || ch.HeritageId != id));
        }



        [HttpGet]
        public async Task<IEnumerable<CulturalHeritageDto>> GetAllCulturalHeritages()
        {
            return await _dbContext.CulturalHeritages
                .Include(ch => ch.CulturalHeritageThemes) // Include join table
                    .ThenInclude(ct => ct.Theme)
                .Include(ch => ch.NationalMinority) // Include National Minority for display
                .Select(ch => new CulturalHeritageDto
                {
                    HeritageID = ch.HeritageId,
                    Name = ch.Name,
                    Description = ch.Description,
                    Location = ch.Location,
                    NationalMinorityID = ch.NationalMinorityId,
                    NationalMinorityName = ch.NationalMinority != null ? ch.NationalMinority.Name : null, // Avoid ?. operator
                    Themes = ch.CulturalHeritageThemes.Select(ct => new ThemeDto
                    {
                        ThemeID = ct.Theme.ThemeId,
                        Name = ct.Theme.Name,
                        Description = ct.Theme.Description
                    }).ToList()
                })
                .ToListAsync();
        }



        [HttpGet("{id}")]
        public async Task<CulturalHeritageDto?> GetCulturalHeritageById(int id)
        {
            var heritage = await _dbContext.CulturalHeritages
                .Include(ch => ch.NationalMinority)
                .FirstOrDefaultAsync(ch => ch.HeritageId == id);

            if (heritage == null) return null;

            return new CulturalHeritageDto
            {
                HeritageID = heritage.HeritageId,
                Name = heritage.Name,
                Description = heritage.Description,
                Location = heritage.Location,
                CreatedAt = heritage.CreatedAt,
                NationalMinorityID = heritage.NationalMinorityId,
                NationalMinorityName = heritage.NationalMinority?.Name // For display
            };
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<int> CreateCulturalHeritage(CulturalHeritageDto culturalHeritageDto)
        {
            var heritage = new CulturalHeritage
            {
                Name = culturalHeritageDto.Name,
                Description = culturalHeritageDto.Description,
                Location = culturalHeritageDto.Location,
                CreatedAt = DateTime.UtcNow,
                NationalMinorityId = culturalHeritageDto.NationalMinorityID // Updated
            };

            _dbContext.CulturalHeritages.Add(heritage);
            await _dbContext.SaveChangesAsync();

            return heritage.HeritageId;
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<bool> UpdateCulturalHeritage(int heritageId, CulturalHeritageDto heritageDto)
        {

            try
            {
                var heritage = await _dbContext.CulturalHeritages.FindAsync(heritageId);
                if (heritage == null) return false;

                heritage.Name = heritageDto.Name;
                heritage.Description = heritageDto.Description;
                heritage.Location = heritageDto.Location;
                heritage.NationalMinorityId = heritageDto.NationalMinorityID;

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during update: {ex.Message}");
                return false;
            }
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
