using Microsoft.EntityFrameworkCore;
using PRN212_SEAL.Contracts.Teams;
using PRN212_SEAL.Entities;
using System.Text.RegularExpressions;

namespace PRN212_SEAL.Services;

public sealed class TeamService : ITeamService
{
    private const int MinMembers = 2;
    private const int MaxMembers = 4;
    private const int MaxTeamNameLength = 100;
    private const int MaxFullNameLength = 100;

    private static readonly Regex StudentCodePattern = new(
        "^[A-Z]{2}[0-9]{6}$", RegexOptions.CultureInvariant);

    private readonly PRN212SealDbContext _dbContext;

    public TeamService(PRN212SealDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<int> CreateTeamAsync(int leaderId, CreateTeamRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (leaderId <= 0)
        {
            throw new ArgumentException("LeaderId không hợp lệ.", nameof(leaderId));
        }

        string teamName = ValidateTeamName(request.TeamName);

        List<CreateTeamMemberRequest> members = ValidateMembers(request.Members);

        Account? leader = await _dbContext.Accounts
            .AsNoTracking()
            .SingleOrDefaultAsync(account => account.Id == leaderId);

        if (leader is null)
        {
            throw new InvalidOperationException("Không tìm thấy tài khoản Leader.");
        }

        if (!string.Equals(leader.Role, "Leader", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Tài khoản không có quyền tạo Team.");
        }

        string leaderStudentCode =
            leader.StudentCode?.Trim() ?? string.Empty;

        if (!StudentCodePattern.IsMatch(leaderStudentCode))
        {
            throw new InvalidOperationException("StudentCode của Leader không hợp lệ.");
        }

        bool leaderAlreadyHasTeam = await _dbContext.Teams
            .AnyAsync(team => team.LeaderId == leaderId);

        if (leaderAlreadyHasTeam)
        {
            throw new InvalidOperationException("Leader đã có Team và không thể tạo thêm.");
        }

        bool teamNameAlreadyExists = await _dbContext.Teams
            .AnyAsync(team => team.TeamName == teamName);

        if (teamNameAlreadyExists)
        {
            throw new InvalidOperationException("Tên Team đã tồn tại.");
        }

        List<string> memberStudentCodes = members.Select(member => member.StudentCode).ToList();

        bool duplicatesLeader = memberStudentCodes.Contains(
            leaderStudentCode, StringComparer.OrdinalIgnoreCase);

        if (duplicatesLeader)
        {
            throw new ArgumentException("StudentCode của thành viên không được trùng với Leader.");
        }

        bool belongsToAnotherAccount = await _dbContext.Accounts
            .AsNoTracking()
            .AnyAsync(account =>
            account.StudentCode != null && memberStudentCodes.Contains(account.StudentCode));

        if (belongsToAnotherAccount)
        {
            throw new InvalidOperationException(
                "Một StudentCode đã thuộc về tài khoản khác.");
        }

        List<string> allStudentCodes = memberStudentCodes.Append(leaderStudentCode).ToList();

        bool alreadyBelongsToTeam = await _dbContext.TeamMembers
            .AsNoTracking().AnyAsync(member => allStudentCodes.Contains(member.StudentCode));

        if (alreadyBelongsToTeam)
        {
            throw new InvalidOperationException("Một StudentCode đã thuộc về Team khác.");
        }

        var team = new Team
        {
            TeamName = teamName,
            LeaderId = leaderId
        };

        team.TeamMembers.Add(new TeamMember
        {
            FullName = leader.FullName.Trim(),
            StudentCode = leaderStudentCode,
            IsLeader = true
        });

        foreach (CreateTeamMemberRequest member in members)
        {
            team.TeamMembers.Add(new TeamMember
            {
                FullName = member.FullName,
                StudentCode = member.StudentCode,
                IsLeader = false
            });
        }

        _dbContext.Teams.Add(team);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException exception)
        {
            _dbContext.ChangeTracker.Clear();

            throw new InvalidOperationException(
                "Không thể tạo Team vì dữ liệu đã thay đổi hoặc bị trùng.", exception);
        }

        return team.Id;
    }

    private static string ValidateTeamName(string? teamName)
    {
        string normalizedTeamName = teamName?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(normalizedTeamName))
        {
            throw new ArgumentException("Tên Team không được để trống.");
        }

        if (normalizedTeamName.Length > MaxTeamNameLength)
        {
            throw new ArgumentException(
                $"Tên Team không được vượt quá {MaxTeamNameLength} ký tự.");
        }

        return normalizedTeamName;
    }

    private static List<CreateTeamMemberRequest> ValidateMembers(
        List<CreateTeamMemberRequest>? members)
    {
        if (members is null)
        {
            throw new ArgumentException("Danh sách thành viên không được để trống.");
        }

        if (members.Count is < MinMembers or > MaxMembers)
        {
            throw new ArgumentException("Team phải có từ 3 đến 5 người, bao gồm Leader.");
        }

        var normalizedMembers = new List<CreateTeamMemberRequest>();
        var studentCodes = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        foreach (CreateTeamMemberRequest member in members)
        {
            if (member is null)
            {
                throw new ArgumentException("Thông tin thành viên không hợp lệ.");
            }

            string fullName = member.FullName?.Trim() ?? string.Empty;
            string studentCode =
                member.StudentCode?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException("Họ tên thành viên không được để trống.");
            }

            if (fullName.Length > MaxFullNameLength)
            {
                throw new ArgumentException($"Họ tên không được vượt quá {MaxFullNameLength} ký tự.");
            }

            if (!StudentCodePattern.IsMatch(studentCode))
            {
                throw new ArgumentException(
                    $"StudentCode '{studentCode}' phải gồm 2 chữ hoa và 6 chữ số.");
            }

            if (!studentCodes.Add(studentCode))
            {
                throw new ArgumentException(
                    $"StudentCode '{studentCode}' bị trùng trong danh sách.");
            }

            normalizedMembers.Add(new CreateTeamMemberRequest
            {
                FullName = fullName,
                StudentCode = studentCode
            });
        }

        return normalizedMembers;
    }
}