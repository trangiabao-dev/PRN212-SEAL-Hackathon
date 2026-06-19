using PRN212_SEAL.Contracts.Teams;
using PRN212_SEAL.Services;
using PRN212_SEAL.State;

namespace PRN212_SEAL.ViewModels;

public sealed class TeamManagementViewModel : ViewModelBase
{
    private readonly ITeamService _teamService;
    private readonly UserSession _userSession;

    private TeamDetailsResponse? _currentTeam;
    private string _errorMessage = string.Empty;
    private bool _isBusy;

    public TeamManagementViewModel(ITeamService teamService, UserSession userSession)
    {
        _teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));

        _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
    }

    public TeamDetailsResponse? CurrentTeam
    {
        get => _currentTeam;
        private set
        {
            _currentTeam = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasTeam));
        }
    }

    public bool HasTeam => CurrentTeam is not null;

    public string ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            _isBusy = value;
            OnPropertyChanged();
        }
    }

    public async Task LoadTeamAsync()
    {
        ErrorMessage = string.Empty;

        var currentAccount = _userSession.CurrentAccount;

        if (currentAccount is null)
        {
            CurrentTeam = null;
            ErrorMessage = "Bạn chưa đăng nhập.";
            return;
        }

        if (!string.Equals(currentAccount.Role, "Leader", StringComparison.Ordinal))
        {
            CurrentTeam = null;
            ErrorMessage = "Chỉ Leader được truy cập chức năng Team.";
            return;
        }

        IsBusy = true;

        try
        {
            CurrentTeam = await _teamService
                .GetTeamByLeaderIdAsync(currentAccount.Id);
        }
        catch (Exception exception)
        {
            CurrentTeam = null;
            ErrorMessage = exception.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}