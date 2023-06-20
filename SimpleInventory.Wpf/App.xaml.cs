using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Microsoft.Extensions.DependencyInjection;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Factories;
using SimpleInventory.Wpf.Properties;
using SimpleInventory.Wpf.Services;
using SimpleInventory.Wpf.ViewModels;
using SimpleInventory.Wpf.ViewModels.PageViewModes;
using System;
using System.Configuration;
using System.Windows;

namespace SimpleInventory.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Services { get; }

        public new static App Current => (App)Application.Current;

        public App()
        {
            Services = CreateServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = Services.GetRequiredService<MainWindow>();
            MainWindow.DataContext = Services.GetRequiredService<MainViewModel>();

            Services.GetService<ISettingsService>()!.SetTheme(Settings.Default.Theme);

            MainWindow.Show();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Settings.Default.Save();
        }

        private IServiceProvider CreateServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<IInventoryService, InventoryService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ICustomerService, CustomerService>();
            services.AddSingleton<IOrderService, OrderService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton(CreateDbConnection());
            services.AddSingleton(CreateMapper());

            services.AddScoped<IViewModelFactory, ViewModelFactory>();
            services.AddScoped<ViewModelBase>();
            services.AddScoped<MainViewModel>();
            services.AddScoped<HomePageViewModel>();
            services.AddScoped<InventoryPageViewModel>();
            services.AddScoped<OrdersPageViewModel>();
            services.AddScoped<CustomersPageViewModel>();
            services.AddScoped<SettingsPageViewModel>();
            services.AddScoped<ItemDetailsViewModel>();
            services.AddScoped<ReceivingViewModel>();
            services.AddScoped<OrderDetailsViewModel>();
            services.AddScoped<CustomerDetailsViewModel>();
            services.AddScoped<MoveInventoryViewModel>();
            services.AddScoped<InventoryEntryDetailsViewModel>();

            return services.BuildServiceProvider();
        }

        private MongoDbConnection CreateDbConnection()
        {
            return new MongoDbConnection(ConfigurationManager.ConnectionStrings["SimpleInventory"].ConnectionString);
        }

        private IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddCollectionMappers();
                cfg.CreateMap<InventoryEntryViewModel, InventoryEntryModel>().ReverseMap();
                cfg.CreateMap<OrderViewModel, OrderModel>().ReverseMap();
                cfg.CreateMap<OrderSummaryViewModel, OrderSummaryModel>().ReverseMap();
                cfg.CreateMap<OrderLineViewModel, OrderLineModel>().ReverseMap();
                cfg.CreateMap<ItemViewModel, ItemModel>().ReverseMap().DisableCtorValidation();
                cfg.CreateMap<CustomerViewModel, CustomerModel>().ReverseMap();
            });
            config.AssertConfigurationIsValid();
            return config.CreateMapper();
        }
    }
}
