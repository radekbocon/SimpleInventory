using SimpleInventory.Core.Extentions;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.Services;
using SimpleInventory.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleInventory.Wpf.Controls.Dialogs
{
    public class PickCustomerViewModel : ViewModelBase
    {
        private readonly ICustomerService _customerService;
        private readonly INavigationService _navigationService;
        private ObservableCollection<CustomerModel> _customers;
        private Action<CustomerModel> _action;
        private CustomerModel _selectedCustomer;
        private ICommand _cancelCommand;
        private ICommand _pickCustomerCommand;
        private ICommand _loadCustomersCommand;


        public ICommand LoadCustomersCommand
        {
            get
            {
                if (_loadCustomersCommand == null)
                {
                    _loadCustomersCommand = new RelayCommand(
                        async p => await GetCustomers(),
                        p => true);
                }

                return _loadCustomersCommand;
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(
                        p => Cancel(),
                        p => true);
                }

                return _cancelCommand;
            }
        }

        public ICommand PickCustomerCommand
        {
            get
            {
                if (_pickCustomerCommand == null)
                {
                    _pickCustomerCommand = new RelayCommand(
                        p => PickCustomer(),
                        p => SelectedCustomer != null);
                }

                return _pickCustomerCommand;
            }
        }

        public CustomerModel SelectedCustomer 
        { 
            get => _selectedCustomer;
            set
            {
                SetProperty(ref _selectedCustomer, value);
            }
        }

        public ObservableCollection<CustomerModel> Customers
        {
            get => _customers;
            set
            {
                SetProperty(ref _customers, value);
            }
        }

        public PickCustomerViewModel(ICustomerService customerService, INavigationService navigationService, Action<CustomerModel> callback)
        {
            _customerService = customerService;
            _navigationService = navigationService;
            _action = callback;
        }

        private async Task GetCustomers()
        {
            var list = await _customerService.GetAll();
            Customers = new ObservableCollection<CustomerModel>(list);
        }

        private void PickCustomer()
        {
            _action.Invoke(SelectedCustomer);
            _navigationService.DialogResult(true);
        }

        private void Cancel()
        {
            _navigationService.DialogResult(false);
        }
    }
}
