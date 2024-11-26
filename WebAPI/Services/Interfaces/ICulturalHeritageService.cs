using WebAPI.DTOs;

namespace WebAPI.Services.Interfaces
{
    public interface ICulturalHeritageService
    {
        Task<IEnumerable<CulturalHeritageDto>> GetAllCulturalHeritages();
        Task<CulturalHeritageDto> GetCulturalHeritageById(int heritageId);
        Task<int> CreateCulturalHeritage(CulturalHeritageDto heritageDto);
        Task<bool> UpdateCulturalHeritage(int heritageId, CulturalHeritageDto heritageDto);
        Task<bool> DeleteCulturalHeritage(int heritageId);
        Task<IEnumerable<CulturalHeritageDto>> SearchCulturalHeritages(string name);
    }
}
