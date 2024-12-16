using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class NationalMinority
{
    public int MinorityId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<CulturalHeritage> CulturalHeritages { get; set; } = new List<CulturalHeritage>();
}
