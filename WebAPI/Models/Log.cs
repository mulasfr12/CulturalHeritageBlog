using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class Log
{
    public int LogId { get; set; }

    public string Message { get; set; } = null!;

    public int UserId { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual User User { get; set; } = null!;
}
