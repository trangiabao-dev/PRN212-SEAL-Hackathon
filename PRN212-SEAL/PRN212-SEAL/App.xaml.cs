using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PRN212_SEAL.Entities;
using PRN212_SEAL.Services;
using PRN212_SEAL.State;
using PRN212_SEAL.ViewModels;
using PRN212_SEAL.Views;

namespace PRN212_SEAL
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Database
            services.AddDbContext<PRN212SealDbContext>();

            // Services
            services.AddSingleton<UserSession>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IAuthService, AuthService>();

            // ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddSingleton<MainViewModel>();

            // Main Window
            services.AddSingleton(s => new MainWindow
            {
                DataContext = s.GetRequiredService<MainViewModel>()
            });
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var navigationService = _serviceProvider.GetRequiredService<INavigationService>();
            navigationService.NavigateTo<LoginViewModel>();

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
