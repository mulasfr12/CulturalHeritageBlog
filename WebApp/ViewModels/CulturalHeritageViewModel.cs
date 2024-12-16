namespace WebApp.ViewModels
{
    public class CulturalHeritageViewModel
    {
        public int HeritageID { get; set; } // ID for edit scenarios
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }

        // Add this property for National Minority selection
        public int? NationalMinorityID { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
