namespace PRN212_SEAL.Contracts.Teams;

public sealed class CreateTeamRequest
{
    public string TeamName { get; set; } = string.Empty;

    public List<CreateTeamMemberRequest> Members { get; set; } = new();
}