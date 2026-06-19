using System;
using PRN212_SEAL.Entities;

namespace PRN212_SEAL.State;

public class UserSession
{
    private Account? _currentAccount;

    public Account? CurrentAccount
    {
        get => _currentAccount;
        set
        {
            _currentAccount = value;
            StateChanged?.Invoke();
        }
    }

    public event Action? StateChanged;
    
    public bool IsLoggedIn => CurrentAccount != null;
    
    public void Logout()
    {
        CurrentAccount = null;
    }
}
