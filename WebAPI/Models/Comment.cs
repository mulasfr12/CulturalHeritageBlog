using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public string Content { get; set; } = null!;

    public int UserId { get; set; }

    public int HeritageId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual CulturalHeritage Heritage { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
