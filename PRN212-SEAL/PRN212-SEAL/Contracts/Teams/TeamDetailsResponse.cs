namespace PRN212_SEAL.Contracts.Teams;

public sealed class TeamDetailsResponse
{
    public int TeamId { get; set; }

    public string TeamName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public List<TeamMemberResponse> Members { get; set; } = new();
}