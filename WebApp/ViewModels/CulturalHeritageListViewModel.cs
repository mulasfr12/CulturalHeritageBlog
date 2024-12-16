using WebAPI.DTOs;

namespace WebApp.ViewModels
{
    public class CulturalHeritageListViewModel
    {
        public IEnumerable<CulturalHeritageDto> Heritages { get; set; }
        public string Search { get; set; }
        public int? NationalMinority { get; set; } // Selected National Minority ID
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        // Add this property to store the list of National Minorities
        public IEnumerable<NationalMinorityDto> NationalMinorities { get; set; }
    }
}
