using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Services.Interfaces;

namespace WebAPI.Services
{
    public class NationalMinorityService : INationalMinorityService
    {
        private readonly CulturalHeritageDbContext _dbContext;

        public NationalMinorityService(CulturalHeritageDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<NationalMinorityDto>> GetAllNationalMinorities()
        {
            return await _dbContext.NationalMinorities
                .Select(nm => new NationalMinorityDto
                {
                    MinorityID = nm.MinorityId,
                    Name = nm.Name,
                    Description = nm.Description
                })
                .ToListAsync();
        }

        public async Task<NationalMinorityDto> GetNationalMinorityById(int id)
        {
            var minority = await _dbContext.NationalMinorities.FindAsync(id);
            if (minority == null) return null;

            return new NationalMinorityDto
            {
                MinorityID = minority.MinorityId,
                Name = minority.Name,
                Description = minority.Description
            };
        }

        public async Task<int> CreateNationalMinority(NationalMinorityDto nationalMinorityDto)
        {
            var minority = new NationalMinority
            {
                Name = nationalMinorityDto.Name,
                Description = nationalMinorityDto.Description
            };

            _dbContext.NationalMinorities.Add(minority);
            await _dbContext.SaveChangesAsync();

            return minority.MinorityId;
        }

        public async Task<bool> UpdateNationalMinority(int id, NationalMinorityDto nationalMinorityDto)
        {
            var minority = await _dbContext.NationalMinorities.FindAsync(id);
            if (minority == null) return false;

            minority.Name = nationalMinorityDto.Name;
            minority.Description = nationalMinorityDto.Description;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNationalMinority(int id)
        {
            var minority = await _dbContext.NationalMinorities.FindAsync(id);
            if (minority == null) return false;

            _dbContext.NationalMinorities.Remove(minority);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
