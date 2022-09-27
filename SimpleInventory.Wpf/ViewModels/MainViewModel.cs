using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.Services;
using SimpleInventory.Wpf.ViewModels.PageViewModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleInventory.Wpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ICommand? _changePageCommand;
        private readonly INavigationService _navigationService;

        private PageViewModel? _currentPageViewModel;
        private List<PageViewModel>? _pageViewModels;

        public MainViewModel(
            INavigationService navigationService,
            HomePageViewModel homePageViewModel, 
            InventoryPageViewModel inventoryPageViewModel,
            OrdersPageViewModel ordersPageViewModel,
            CustomersPageViewModel customersPageViewModel,
            SettingsPageViewModel settingsPageViewModel)
        {
            _navigationService = navigationService;
            PageViewModels.Add(homePageViewModel);
            PageViewModels.Add(inventoryPageViewModel);
            PageViewModels.Add(ordersPageViewModel);
            PageViewModels.Add(customersPageViewModel);
            PageViewModels.Add(settingsPageViewModel);

            ChangePageCommand.Execute(homePageViewModel);
        }

        public ICommand ChangePageCommand
        {
            get
            {
                if (_changePageCommand == null)
                {
                    _changePageCommand = new RelayCommand(
                        p => ChangeViewModel((PageViewModel)p),
                        p => p is PageViewModel);
                }

                return _changePageCommand;
            }
        }

        public List<PageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<PageViewModel>();

                return _pageViewModels;
            }
        }

        public PageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    SetActive(false);
                    _currentPageViewModel = value;
                    SetActive(true);
                    NotifyPropertyChanged(nameof(CurrentPageViewModel));
                }
            }
        }

        private void SetActive(bool active)
        {
            if (_currentPageViewModel != null)
            {
                _currentPageViewModel.IsActive = active;
            }
        }

        private void ChangeViewModel(PageViewModel viewModel)
        {
            _navigationService.OpenPage(viewModel);
            CurrentPageViewModel = viewModel;
        }
    }
}
