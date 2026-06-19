using PRN212_SEAL.ViewModels;
using System;

namespace PRN212_SEAL.Services;

public interface INavigationService
{
    ViewModelBase CurrentViewModel { get; }
    event Action? CurrentViewModelChanged;
    void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
}
