using Microsoft.Extensions.DependencyInjection;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.Services;
using SimpleInventory.Wpf.ViewModels;
using SimpleInventory.Wpf.ViewModels.PageViewModes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleInventory.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            IServiceProvider serviceProvider = CreateServiceProvider();

            MainWindow = serviceProvider.GetRequiredService<MainWindow>();
            MainWindow.DataContext = serviceProvider.GetRequiredService<MainViewModel>();

            MainWindow.Show();
            base.OnStartup(e);
        }

        private IServiceProvider CreateServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<IInventoryService, InventoryService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ICustomerService, CustomerService>();
            services.AddSingleton<IOrderService, OrderService>();

            services.AddScoped<ViewModelBase>();
            services.AddScoped<MainViewModel>();
            services.AddScoped<HomePageViewModel>();
            services.AddScoped<InventoryPageViewModel>();
            services.AddScoped<OrdersPageViewModel>();
            services.AddScoped<CustomersPageViewModel>();
            services.AddScoped<SettingsPageViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
