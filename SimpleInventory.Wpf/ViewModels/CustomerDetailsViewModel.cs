using Bogus;
using SimpleInventory.Core.Extentions;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleInventory.Wpf.ViewModels
{
    public class CustomerDetailsViewModel : ViewModelBase
    {
        private readonly ICustomerService _customerService;
        private readonly IDialogService _dialogService;

        private CustomerModel _customer;
        private ICommand _saveCommand;
        private ICommand _cancelCommand;
        private ICommand _addNewAddress;
        private ICommand _deleteAddress;

        public CustomerDetailsViewModel(string customerId, ICustomerService customerService, IDialogService dialogService)
        {
            _customerService = customerService;
            _dialogService = dialogService;
            SetCustomer(customerId).Await();
        }

        public CustomerDetailsViewModel(ICustomerService customerService, IDialogService dialogService)
        {
            _customerService = customerService;
            _dialogService = dialogService;
            Customer = new CustomerModel();
        }

        private AddressModel _selectedAddress;

        public AddressModel? SelectedAddress
        {
            get
            {
                if (Customer?.Addresses.Count < 1)
                {
                    Customer.Addresses = new List<AddressModel>();
                    Customer.Addresses.Add(new AddressModel());
                }
                if (_selectedAddress == null)
                {
                    _selectedAddress = Customer?.Addresses[0];
                }
                return _selectedAddress;
            }
            set { SetProperty(ref _selectedAddress, value); }
        }


        public CustomerModel Customer
        {
            get => _customer;
            set
            {
                SetProperty(ref _customer, value);
                NotifyPropertyChanged(nameof(SelectedAddress));
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

        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(
                        async p => await Save(),
                        p => p is CustomerModel);
                }

                return _saveCommand;
            }
        }

        public ICommand AddNewAddress
        {
            get
            {
                if (_addNewAddress == null)
                {
                    _addNewAddress = new RelayCommand(
                        p => AddAddress(),
                        p => true);
                }

                return _addNewAddress;
            }
        }

        public ICommand DeleteAddress
        {
            get
            {
                if (_deleteAddress == null)
                {
                    _deleteAddress = new RelayCommand(
                        p => DeleteSelectedAddress(),
                        p => true);
                }

                return _deleteAddress;
            }
        }

        private void DeleteSelectedAddress()
        {
            if (SelectedAddress == null) return;
            Customer?.Addresses.Remove(SelectedAddress);
            SelectedAddress = Customer?.Addresses?.FirstOrDefault();
            NotifyPropertyChanged(nameof(Customer.Addresses));
        }

        private void AddAddress()
        {
            var address = new AddressModel();
            Customer.Addresses.Add(address);
            SelectedAddress = address;
            NotifyPropertyChanged(nameof(Customer.Addresses));
        }

        private void Cancel()
        {
            _dialogService.DialogResult(false);
        }

        private async Task Save()
        {
            await _customerService.UpsertOne(Customer);
            _dialogService.DialogResult(true);
        }

        private async Task SetCustomer(string id)
        {
            Customer = await _customerService.GetById(id);
        }
    }
}
