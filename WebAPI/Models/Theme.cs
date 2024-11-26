using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class Theme
{
    public int ThemeID { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<CulturalHeritageTheme> CulturalHeritageThemes { get; set; } = new List<CulturalHeritageTheme>();

}
