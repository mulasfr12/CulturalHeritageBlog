using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class CulturalHeritageTheme
{
    public int HeritageId { get; set; }

    public int ThemeId { get; set; }

    public string? Description { get; set; }

    public virtual CulturalHeritage Heritage { get; set; } = null!;

    public virtual Theme Theme { get; set; } = null!;
}
