using PRN212_SEAL.Contracts.Teams;

namespace PRN212_SEAL.Services
{
    public interface ITeamService
    {
        Task<int> CreateTeamAsync(int leaderId, CreateTeamRequest request);
    }
}
