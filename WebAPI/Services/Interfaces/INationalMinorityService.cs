using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.DTOs;

namespace WebAPI.Services.Interfaces
{
    public interface INationalMinorityService
    {
        Task<IEnumerable<NationalMinorityDto>> GetAllNationalMinorities();
        Task<NationalMinorityDto> GetNationalMinorityById(int id);
        Task<int> CreateNationalMinority(NationalMinorityDto nationalMinorityDto);
        Task<bool> UpdateNationalMinority(int id, NationalMinorityDto nationalMinorityDto);
        Task<bool> DeleteNationalMinority(int id);
    }
}
