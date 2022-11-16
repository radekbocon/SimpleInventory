﻿using AutoMapper;
using Bogus;
using SimpleInventory.Core.Extentions;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Commands;
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

        private CustomerViewModel _customer;
        private CustomerViewModel _customerBackup;
        private ICommand _cancelCommand;
        private ICommand _saveCommand;
        private ICommand _addNewAddress;
        private ICommand _deleteAddress;

        public CustomerDetailsViewModel(string customerId, ICustomerService customerService, INavigationService navigationService, IMapper mapper)
        {
            _customerService = customerService;
            _navigationService = navigationService;
            _mapper = mapper;
            SetCustomer(customerId).Await();
        }

        public CustomerDetailsViewModel(ICustomerService customerService, INavigationService dialogService, IMapper mapper)
        {
            _customerService = customerService;
            _navigationService = dialogService;
            _mapper = mapper;
            Customer = new CustomerViewModel();
            _customerBackup = new CustomerViewModel();
        }

        public string Name { get; set; } = "Customer Details";

        private AddressModel _selectedAddress;

        public AddressModel? SelectedAddress
        {
            get
            {
                
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
            await SetCustomer(Customer.Id);
            _navigationService.ModalResult(true);
        }

        private void CancelChanges()
        {
            Customer = new CustomerViewModel(_customerBackup);
        }

        private async Task SetCustomer(string id)
        {
            var customerModel = await _customerService.GetById(id);
            Customer = _mapper.Map<CustomerViewModel>(customerModel);
            _customerBackup = new CustomerViewModel(_customer);
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
