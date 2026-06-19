using System;
using System.Collections.Generic;

namespace PRN212_SEAL.Entities;

public partial class Score
{
    public int Id { get; set; }

    public int SubmissionId { get; set; }

    public int JudgeId { get; set; }

    public decimal ScoreValue { get; set; }

    public string? Comment { get; set; }

    public DateTime ScoredAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Account Judge { get; set; } = null!;

    public virtual Submission Submission { get; set; } = null!;
}
