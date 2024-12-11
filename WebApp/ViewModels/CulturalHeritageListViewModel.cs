using WebAPI.DTOs;

namespace WebApp.ViewModels
{
    public class CulturalHeritageListViewModel
    {
        public IEnumerable<CulturalHeritageDto> Heritages { get; set; }
        public string Search { get; set; }
        public int? NationalMinority { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
