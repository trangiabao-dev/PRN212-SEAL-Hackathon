using System;
using System.Collections.Generic;

namespace PRN212_SEAL.Entities;

public partial class TeamMember
{
    public int Id { get; set; }

    public int TeamId { get; set; }

    public string FullName { get; set; } = null!;

    public string StudentCode { get; set; } = null!;

    public bool IsLeader { get; set; }

    public virtual Team Team { get; set; } = null!;
}
