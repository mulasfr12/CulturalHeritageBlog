using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class HeritageImage
{
    public int ImageId { get; set; }

    public int HeritageId { get; set; }

    public string ImagePath { get; set; } = null!;

    public DateTime UploadedAt { get; set; }

    public virtual CulturalHeritage Heritage { get; set; } = null!;
}
