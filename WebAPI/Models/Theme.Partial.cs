namespace WebAPI.Models
{
    public partial class Theme
    {
       public virtual ICollection<CulturalHeritage> CulturalHeritages { get; set; }
    }
}
