using System.Collections.Generic;
using System.Threading.Tasks;
using PRN212_SEAL.Contracts.Teams;

namespace PRN212_SEAL.Services;

public interface ITeamService
{
    Task<int> CreateTeamAsync(int leaderId, CreateTeamRequest request);
    Task<TeamDetailsResponse?> GetTeamByLeaderIdAsync(int leaderId);

    // Bổ sung API cho Giám khảo chấm thi
    Task<List<TeamDetailsResponse>> GetAllTeamsAsync();
}
