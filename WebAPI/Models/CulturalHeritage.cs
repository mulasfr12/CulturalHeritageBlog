using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class CulturalHeritage
{
    public int HeritageId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Location { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? NationalMinorityId { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<CulturalHeritageTheme> CulturalHeritageThemes { get; set; } = new List<CulturalHeritageTheme>();

    public virtual ICollection<HeritageImage> HeritageImages { get; set; } = new List<HeritageImage>();

    public virtual NationalMinority? NationalMinority { get; set; }
}
