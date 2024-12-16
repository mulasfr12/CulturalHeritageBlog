namespace WebAPI.DTOs
{
    public class CulturalHeritageDto
    {
        public int HeritageID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? NationalMinorityID { get; set; } // Updated field
        public string NationalMinorityName { get; set; } // Optional for display purposes
        public List<ThemeDto> Themes { get; set; } = new List<ThemeDto>();

    }
}
