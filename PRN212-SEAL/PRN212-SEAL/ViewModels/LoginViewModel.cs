using System.Threading.Tasks;
using System.Windows.Input;
using PRN212_SEAL.Commands;
using PRN212_SEAL.Services;
using PRN212_SEAL.State;

namespace PRN212_SEAL.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly UserSession _userSession;

    private string _username = string.Empty;
    public string Username
    {
        get => _username;
        set { _username = value; OnPropertyChanged(); }
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); }
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set { _errorMessage = value; OnPropertyChanged(); }
    }

    public ICommand LoginCommand { get; }
    public ICommand NavigateToRegisterCommand { get; }

    public LoginViewModel(IAuthService authService, INavigationService navigationService, UserSession userSession)
    {
        _authService = authService;
        _navigationService = navigationService;
        _userSession = userSession;

        LoginCommand = new RelayCommand(async _ => await LoginAsync(), _ => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password));
        NavigateToRegisterCommand = new RelayCommand(_ => _navigationService.NavigateTo<RegisterViewModel>());
    }

    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;
        var account = await _authService.LoginAsync(Username, Password);
        
        if (account == null)
        {
            ErrorMessage = "Sai tài khoản hoặc mật khẩu.";
            return;
        }

        _userSession.CurrentAccount = account;
        
        // MVP: After login, you would navigate to LeaderDashboard or JudgeDashboard.
        // As those are in other team members' scope, we just set session here.
        ErrorMessage = $"Đăng nhập thành công với vai trò: {account.Role}";
    }
}
