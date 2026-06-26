using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PRN212_SEAL.Commands;
using PRN212_SEAL.Contracts.Teams;
using PRN212_SEAL.Services;
using PRN212_SEAL.State;

namespace PRN212_SEAL.ViewModels;

public sealed class TeamManagementViewModel : ViewModelBase
{
    private const int MinRegularMembers = 2;
    private const int MaxRegularMembers = 4;

    private readonly ITeamService _teamService;
    private readonly UserSession _userSession;

    private TeamDetailsResponse? _currentTeam;
    private string _teamName = string.Empty;
    private string _errorMessage = string.Empty;
    private string _successMessage = string.Empty;
    private bool _isBusy;

    public TeamManagementViewModel(ITeamService teamService, UserSession userSession)
    {
        _teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
        _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));

        Members.Add(new CreateTeamMemberRequest());
        Members.Add(new CreateTeamMemberRequest());

        AddMemberCommand = new RelayCommand(
            _ => AddMember(),
            _ => !IsBusy && !HasTeam && Members.Count < MaxRegularMembers);

        RemoveMemberCommand = new RelayCommand(
            RemoveMember,
            p => !IsBusy && !HasTeam && Members.Count > MinRegularMembers && p is CreateTeamMemberRequest);

        CreateTeamCommand = new RelayCommand(
            async _ => await CreateTeamAsync(),
            _ => !IsBusy && !HasTeam);
    }

    public ObservableCollection<CreateTeamMemberRequest> Members { get; } = new();
    public ICommand AddMemberCommand { get; }
    public ICommand RemoveMemberCommand { get; }
    public ICommand CreateTeamCommand { get; }

    public TeamDetailsResponse? CurrentTeam
    {
        get => _currentTeam;
        private set
        {
            _currentTeam = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasTeam));
            RefreshCommands();
        }
    }

    public bool HasTeam => CurrentTeam is not null;

    public string TeamName
    {
        get => _teamName;
        set { _teamName = value; OnPropertyChanged(); }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set { _errorMessage = value; OnPropertyChanged(); }
    }

    public string SuccessMessage
    {
        get => _successMessage;
        private set { _successMessage = value; OnPropertyChanged(); }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set { _isBusy = value; OnPropertyChanged(); RefreshCommands(); }
    }

    public async Task LoadTeamAsync()
    {
        ClearMessages();
        int? leaderId = GetCurrentLeaderId();
        if (leaderId is null) { CurrentTeam = null; return; }

        IsBusy = true;
        try
        {
            CurrentTeam = await _teamService.GetTeamByLeaderIdAsync(leaderId.Value);
        }
        catch (Exception ex)
        {
            CurrentTeam = null;
            ErrorMessage = ex.Message;
        }
        finally { IsBusy = false; }
    }

    private async Task CreateTeamAsync()
    {
        ClearMessages();
        int? leaderId = GetCurrentLeaderId();
        if (leaderId is null) return;

        IsBusy = true;
        try
        {
            var req = new CreateTeamRequest
            {
                TeamName = TeamName,
                Members = Members.Select(m => new CreateTeamMemberRequest
                {
                    FullName = m.FullName,
                    StudentCode = m.StudentCode
                }).ToList()
            };

            int teamId = await _teamService.CreateTeamAsync(leaderId.Value, req);
            CurrentTeam = await _teamService.GetTeamByLeaderIdAsync(leaderId.Value);

            if (CurrentTeam is null)
                throw new InvalidOperationException("Đã tạo Team nhưng không thể tải lại dữ liệu.");

            TeamName = string.Empty;
            SuccessMessage = $"Tạo Team thành công! Mã nhóm: {teamId}.";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally { IsBusy = false; }
    }

    private void AddMember()
    {
        if (Members.Count >= MaxRegularMembers)
        {
            ErrorMessage = "Chỉ được đăng ký tối đa 4 thành viên thường.";
            return;
        }
        ErrorMessage = string.Empty;
        Members.Add(new CreateTeamMemberRequest());
        RefreshCommands();
    }

    private void RemoveMember(object? p)
    {
        if (Members.Count <= MinRegularMembers)
        {
            ErrorMessage = "Đội thi phải có ít nhất 2 thành viên thường (+1 Leader = 3 người).";
            return;
        }
        if (p is not CreateTeamMemberRequest m) return;

        ErrorMessage = string.Empty;
        Members.Remove(m);
        RefreshCommands();
    }

    private int? GetCurrentLeaderId()
    {
        var acc = _userSession.CurrentAccount;
        if (acc is null) { ErrorMessage = "Bạn chưa đăng nhập."; return null; }
        if (!string.Equals(acc.Role, "Leader", StringComparison.Ordinal))
        {
            ErrorMessage = "Chỉ Trưởng nhóm (Leader) mới được truy cập module này.";
            return null;
        }
        return acc.Id;
    }

    private void ClearMessages() { ErrorMessage = string.Empty; SuccessMessage = string.Empty; }
    private static void RefreshCommands() { CommandManager.InvalidateRequerySuggested(); }
}
