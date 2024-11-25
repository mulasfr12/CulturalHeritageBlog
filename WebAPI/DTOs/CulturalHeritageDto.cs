namespace WebAPI.DTOs
{
    public class CulturalHeritageDto
    {
        public int HeritageID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string NationalMinority { get; set; }
        public List<ThemeDto> Themes { get; set; }
    }
}
