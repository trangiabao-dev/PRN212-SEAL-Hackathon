using System;
using System.Collections.Generic;

namespace PRN212_SEAL.Entities;

public partial class Submission
{
    public int Id { get; set; }

    public int TeamId { get; set; }

    public int ChallengeId { get; set; }

    public string GithubUrl { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime SubmittedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Challenge Challenge { get; set; } = null!;

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();

    public virtual Team Team { get; set; } = null!;
}
