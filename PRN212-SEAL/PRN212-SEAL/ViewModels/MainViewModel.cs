using System;
using PRN212_SEAL.Services;

namespace PRN212_SEAL.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    public ViewModelBase CurrentViewModel => _navigationService.CurrentViewModel;

    public MainViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        _navigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;
    }

    private void OnCurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentViewModel));
    }
}
