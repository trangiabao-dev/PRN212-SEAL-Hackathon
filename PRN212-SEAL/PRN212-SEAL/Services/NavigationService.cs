using PRN212_SEAL.ViewModels;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace PRN212_SEAL.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private ViewModelBase _currentViewModel = null!;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        private set
        {
            _currentViewModel = value;
            CurrentViewModelChanged?.Invoke();
        }
    }

    public event Action? CurrentViewModelChanged;

    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        CurrentViewModel = _serviceProvider.GetRequiredService<TViewModel>();
    }
}
