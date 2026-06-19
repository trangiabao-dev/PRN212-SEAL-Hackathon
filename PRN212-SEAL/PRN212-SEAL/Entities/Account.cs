using System;
using System.Collections.Generic;

namespace PRN212_SEAL.Entities;

public partial class Account
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? StudentCode { get; set; }

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();

    public virtual Team? Team { get; set; }
}
