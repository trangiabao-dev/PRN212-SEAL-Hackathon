using System.Threading.Tasks;
using System.Windows.Input;
using PRN212_SEAL.Commands;
using PRN212_SEAL.Services;

namespace PRN212_SEAL.ViewModels;

public class RegisterViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

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

    private string _fullName = string.Empty;
    public string FullName
    {
        get => _fullName;
        set { _fullName = value; OnPropertyChanged(); }
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(); }
    }

    // Default to Leader for registration demo
    private string _role = "Leader";
    public string Role
    {
        get => _role;
        set { _role = value; OnPropertyChanged(); }
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set { _errorMessage = value; OnPropertyChanged(); }
    }

    private string _successMessage = string.Empty;
    public string SuccessMessage
    {
        get => _successMessage;
        set { _successMessage = value; OnPropertyChanged(); }
    }

    public ICommand RegisterCommand { get; }
    public ICommand NavigateToLoginCommand { get; }

    public RegisterViewModel(IAuthService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;

        RegisterCommand = new RelayCommand(async _ => await RegisterAsync(), _ => CanRegister());
        NavigateToLoginCommand = new RelayCommand(_ => _navigationService.NavigateTo<LoginViewModel>());
    }

    private bool CanRegister()
    {
        return !string.IsNullOrEmpty(Username) && 
               !string.IsNullOrEmpty(Password) &&
               !string.IsNullOrEmpty(Email) &&
               !string.IsNullOrEmpty(FullName);
    }

    private async Task RegisterAsync()
    {
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        var result = await _authService.RegisterAsync(Username, Password, Email, FullName, Role);
        
        if (!result.Success)
        {
            ErrorMessage = result.ErrorMessage;
        }
        else
        {
            SuccessMessage = "Đăng ký thành công! Vui lòng quay lại đăng nhập.";
            Username = string.Empty;
            Password = string.Empty;
            Email = string.Empty;
            FullName = string.Empty;
        }
    }
}
