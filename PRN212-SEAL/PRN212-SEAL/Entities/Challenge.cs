using System;
using System.Collections.Generic;

namespace PRN212_SEAL.Entities;

public partial class Challenge
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Deadline { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
