namespace WebAPI.Models
{
    public partial class CulturalHeritage
    {
        public virtual ICollection<Theme> Themes { get; set; }
    }
}
