namespace PRN212_SEAL.Contracts.Teams;

public sealed class TeamMemberResponse
{
    public string FullName { get; set; } = string.Empty;

    public string StudentCode { get; set; } = string.Empty;

    public bool IsLeader { get; set; }
}