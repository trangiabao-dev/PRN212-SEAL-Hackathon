using System;
using System.Collections.Generic;

namespace PRN212_SEAL.Entities;

public partial class Team
{
    public int Id { get; set; }

    public string TeamName { get; set; } = null!;

    public int LeaderId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Account Leader { get; set; } = null!;

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}
