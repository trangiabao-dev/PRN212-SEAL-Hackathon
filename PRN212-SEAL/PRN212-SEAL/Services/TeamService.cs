using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PRN212_SEAL.Contracts.Teams;
using PRN212_SEAL.Entities;

namespace PRN212_SEAL.Services;

public sealed class TeamService : ITeamService
{
    private const int MinTotalTeamSize = 3;
    private const int MaxTotalTeamSize = 5;
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
        if (leaderId <= 0) throw new ArgumentException("LeaderId không hợp lệ.", nameof(leaderId));

        string teamName = ValidateTeamName(request.TeamName);
        List<CreateTeamMemberRequest> regularMembers = ValidateRegularMembers(request.Members);

        Account? leader = await _dbContext.Accounts
            .AsNoTracking()
            .SingleOrDefaultAsync(a => a.Id == leaderId);

        if (leader is null) throw new InvalidOperationException("Không tìm thấy tài khoản Leader.");
        if (!string.Equals(leader.Role, "Leader", StringComparison.Ordinal))
            throw new InvalidOperationException("Tài khoản không có quyền tạo Team.");

        string leaderStudentCode = leader.StudentCode?.Trim() ?? string.Empty;
        if (!StudentCodePattern.IsMatch(leaderStudentCode))
            throw new InvalidOperationException("StudentCode của Leader không hợp lệ (VD chuẩn: SE123456).");

        if (await _dbContext.Teams.AsNoTracking().AnyAsync(t => t.LeaderId == leaderId))
            throw new InvalidOperationException("Leader đã có Team và không thể tạo thêm.");

        if (await _dbContext.Teams.AsNoTracking().AnyAsync(t => t.TeamName == teamName))
            throw new InvalidOperationException("Tên Team đã tồn tại trong hệ thống.");

        List<string> memberCodes = regularMembers.Select(m => m.StudentCode).ToList();
        if (memberCodes.Any(c => string.Equals(c, leaderStudentCode, StringComparison.OrdinalIgnoreCase)))
            throw new ArgumentException("StudentCode của thành viên không được trùng với Leader.");

        if (await _dbContext.Accounts.AsNoTracking().AnyAsync(a => a.StudentCode != null && memberCodes.Contains(a.StudentCode)))
            throw new InvalidOperationException("Một StudentCode trong danh sách đã thuộc về tài khoản khác.");

        List<string> allCodes = memberCodes.Append(leaderStudentCode).ToList();
        if (await _dbContext.TeamMembers.AsNoTracking().AnyAsync(m => allCodes.Contains(m.StudentCode)))
            throw new InvalidOperationException("Một StudentCode đã tham gia Team khác.");

        var team = new Team { TeamName = teamName, LeaderId = leaderId };

        team.TeamMembers.Add(new TeamMember
        {
            FullName = leader.FullName.Trim(),
            StudentCode = leaderStudentCode,
            IsLeader = true
        });

        foreach (var m in regularMembers)
        {
            team.TeamMembers.Add(new TeamMember
            {
                FullName = m.FullName,
                StudentCode = m.StudentCode,
                IsLeader = false
            });
        }

        _dbContext.Teams.Add(team);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new InvalidOperationException("Xung đột đồng thời: Mã sinh viên hoặc Tên nhóm đã tồn tại.", ex);
        }

        return team.Id;
    }

    public async Task<TeamDetailsResponse?> GetTeamByLeaderIdAsync(int leaderId)
    {
        if (leaderId <= 0) throw new ArgumentException("LeaderId không hợp lệ.", nameof(leaderId));
        return await MapTeamQuery(_dbContext.Teams.Where(t => t.LeaderId == leaderId)).SingleOrDefaultAsync();
    }

    public async Task<List<TeamDetailsResponse>> GetAllTeamsAsync()
    {
        return await MapTeamQuery(_dbContext.Teams).ToListAsync();
    }

    private static IQueryable<TeamDetailsResponse> MapTeamQuery(IQueryable<Team> source)
    {
        return source.AsNoTracking().Select(team => new TeamDetailsResponse
        {
            TeamId = team.Id,
            TeamName = team.TeamName,
            CreatedAt = team.CreatedAt,
            Members = team.TeamMembers
                .OrderByDescending(m => m.IsLeader)
                .ThenBy(m => m.FullName)
                .Select(m => new TeamMemberResponse
                {
                    FullName = m.FullName,
                    StudentCode = m.StudentCode,
                    IsLeader = m.IsLeader
                }).ToList()
        });
    }

    private static string ValidateTeamName(string? name)
    {
        string norm = name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(norm)) throw new ArgumentException("Tên Team không được để trống.");
        if (norm.Length > MaxTeamNameLength) throw new ArgumentException($"Tên Team không vượt quá {MaxTeamNameLength} ký tự.");
        return norm;
    }

    private static List<CreateTeamMemberRequest> ValidateRegularMembers(List<CreateTeamMemberRequest>? members)
    {
        if (members is null) throw new ArgumentException("Danh sách thành viên không được để trống.");

        int totalSize = members.Count + 1;
        if (totalSize < MinTotalTeamSize || totalSize > MaxTotalTeamSize)
            throw new ArgumentException($"Team phải có tổng từ {MinTotalTeamSize} đến {MaxTotalTeamSize} người (tính cả Leader).");

        var result = new List<CreateTeamMemberRequest>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var m in members)
        {
            if (m is null) throw new ArgumentException("Thông tin thành viên không hợp lệ.");
            string fn = m.FullName?.Trim() ?? string.Empty;
            string sc = m.StudentCode?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(fn)) throw new ArgumentException("Họ tên thành viên không được để trống.");
            if (fn.Length > MaxFullNameLength) throw new ArgumentException($"Họ tên không vượt quá {MaxFullNameLength} ký tự.");
            if (!StudentCodePattern.IsMatch(sc)) throw new ArgumentException($"StudentCode '{sc}' phải gồm 2 chữ hoa và 6 chữ số (VD: SE123456).");
            if (!seen.Add(sc)) throw new ArgumentException($"StudentCode '{sc}' bị nhập trùng trong danh sách.");

            result.Add(new CreateTeamMemberRequest { FullName = fn, StudentCode = sc });
        }

        return result;
    }
}