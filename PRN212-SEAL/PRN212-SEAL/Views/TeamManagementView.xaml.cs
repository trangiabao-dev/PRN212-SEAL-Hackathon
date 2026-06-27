using System.Windows;
using System.Windows.Controls;
using PRN212_SEAL.ViewModels;

namespace PRN212_SEAL.Views;

public partial class TeamManagementView : UserControl
{
    public TeamManagementView()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is TeamManagementViewModel viewModel)
        {
            await viewModel.LoadTeamAsync();
        }
    }
}
