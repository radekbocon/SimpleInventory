using AutoMapper;
using Bogus;
using Microsoft.Extensions.DependencyInjection;
using SimpleInventory.Core.Extentions;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.Controls;
using SimpleInventory.Wpf.Services;
using System;
using System.Collections;
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
        private readonly INavigationService _navigationService;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        private CustomerViewModel _customer;
        private CustomerViewModel _customerBackup;
        private ICommand _cancelCommand;
        private ICommand _saveCommand;
        private ICommand _addNewAddress;
        private ICommand _deleteAddress;
        private Action _onCustomerSaved;

        public string Name { get; set; } = "Customer Details";

        private AddressModel _selectedAddress;

        public AddressModel? SelectedAddress
        {
            get
            {
                if (_selectedAddress == null)
                {
                    _selectedAddress = Customer?.Addresses.FirstOrDefault();
                }
                return _selectedAddress;
            }
            set { SetProperty(ref _selectedAddress, value); }
        }


        public CustomerViewModel Customer
        {
            get => _customer;
            set
            {
                SetProperty(ref _customer, value);
                NotifyPropertyChanged(nameof(SelectedAddress));
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
                        p => HasCustomerChanged());
                }

                return _saveCommand;
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(
                        p => CancelChanges(),
                        p => HasCustomerChanged());
                }

                return _cancelCommand;
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

        public CustomerDetailsViewModel()
        {
            _customerService = App.Current.Services.GetRequiredService<ICustomerService>();
            _navigationService = App.Current.Services.GetRequiredService<INavigationService>();
            _mapper = App.Current.Services.GetRequiredService<IMapper>();
            _notificationService = App.Current.Services.GetRequiredService<INotificationService>();
        }

        public CustomerDetailsViewModel Initialize(Action onCustomerSaved, string customerId = null)
        {
            SetCustomer(customerId).Await();
            _onCustomerSaved = onCustomerSaved;
            return this;
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

        private async Task Save()
        {
            var customerModel = _mapper.Map<CustomerModel>(Customer);
            await _customerService.UpsertOne(customerModel);
            _customerBackup = new CustomerViewModel(Customer);
            _onCustomerSaved.Invoke();
            _navigationService.CloseModal();
            _notificationService.Show("Saved", "Customer succesfully saved.", NotificationType.Info);
        }

        private void CancelChanges()
        {
            Customer = new CustomerViewModel(_customerBackup);
        }

        private async Task SetCustomer(string id)
        {
            if (id == null)
            {
                Customer = new CustomerViewModel();
                _customerBackup = new CustomerViewModel();
            }
            else
            {
                var customerModel = await _customerService.GetById(id);
                Customer = _mapper.Map<CustomerViewModel>(customerModel);
                _customerBackup = new CustomerViewModel(_customer);
            }
        }

        private bool HasCustomerChanged()
        {
            if (_customer == null || _customerBackup == null)
            {
                return false;
            }
            var hasCHanged = !Customer.Equals(_customerBackup);
            return hasCHanged;
        }
    }
}
