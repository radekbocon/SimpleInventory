using SimpleInventory.Wpf.Commands;
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
        private ICommand _changePageCommand;

        private PageViewModel _currentPageViewModel;
        private List<PageViewModel> _pageViewModels;

        public MainViewModel(
            HomePageViewModel homePageViewModel, 
            InventoryPageViewModel inventoryPageViewModel,
            OrdersPageViewModel ordersPageViewModel,
            CustomersPageViewModel customersPageViewModel,
            SettingsPageViewModel settingsPageViewModel)
        {
            PageViewModels.Add(homePageViewModel);
            PageViewModels.Add(inventoryPageViewModel);
            PageViewModels.Add(ordersPageViewModel);
            PageViewModels.Add(customersPageViewModel);
            PageViewModels.Add(settingsPageViewModel);

            CurrentPageViewModel = PageViewModels[0];
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
                    RaisePropertyChanged(nameof(CurrentPageViewModel));
                }
            }
        }

        private bool _isDialogOpen;

        public bool IsDialogOpen
        {
            get { return _isDialogOpen; }
            set { SetProperty(ref _isDialogOpen, value); }
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
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels.FirstOrDefault(vm => vm == viewModel);
        }
    }
}
