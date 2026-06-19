namespace PRN212_SEAL.Contracts.Teams;

public sealed class CreateTeamMemberRequest
{
    public string FullName { get; set; } = string.Empty;

    public string StudentCode { get; set; } = string.Empty;
}