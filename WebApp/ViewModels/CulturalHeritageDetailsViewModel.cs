namespace WebApp.ViewModels
{
    public class CulturalHeritageDetailsViewModel
    {
        public int HeritageID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string NationalMinorityName { get; set; }
        public List<ThemeViewModel> Themes { get; set; } = new List<ThemeViewModel>();
        public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
    }
    
}
