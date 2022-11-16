using MongoDB.Bson.Serialization.Attributes;
using SimpleInventory.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Wpf.ViewModels
{
    public class CustomerViewModel : ViewModelBase
    {
        private ObservableCollection<AddressModel> _addresses = new();
        private string? _firstName;
        private string? _lastName;
        private string? _email;
        private string? _phoneNumber;
        private string? _companyName;

        public string? Id { get; set; }
        public string? CompanyName 
        { 
            get => _companyName; 
            set => SetProperty(ref _companyName, value);
        }
        public string? FirstName 
        { 
            get => _firstName;
            set
            {
                SetProperty(ref _firstName, value);
                NotifyPropertyChanged(nameof(FullName));
            }
        }
        public string? LastName 
        { 
            get => _lastName;
            set
            {
                SetProperty(ref _lastName, value);
                NotifyPropertyChanged(nameof(FullName));
            }
        }
        public string? Email 
        { 
            get => _email;
            set => SetProperty(ref _email, value);
        }
        public string? PhoneNumber 
        { 
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }
        public ObservableCollection<AddressModel> Addresses
        {
            get => _addresses;
            set
            {
                SetProperty(ref _addresses, value);
            }
        }
        public string? FullName => $"{FirstName} {LastName}";
        public string? Location
        {
            get
            {
                if (Addresses.Count > 0)
                {
                    return $"{Addresses[0].City}, {Addresses[0].Country}";
                }
                return null;
            }
        }

        public CustomerViewModel(CustomerViewModel customer)
        {
            Id = customer.Id;
            CompanyName = customer.CompanyName;
            FirstName = customer.FirstName;
            LastName = customer.LastName;
            Email = customer.Email;
            PhoneNumber = customer.PhoneNumber;
            foreach (var address in customer.Addresses)
            {
                Addresses.Add(new AddressModel(address));
            }
        }

        public CustomerViewModel()
        {

        }

        public override bool Equals(object? obj)
        {
            return obj is CustomerViewModel model &&
                   Id == model.Id &&
                   CompanyName == model.CompanyName &&
                   FirstName == model.FirstName &&
                   LastName == model.LastName &&
                   Email == model.Email &&
                   AreAddressesEqual(model.Addresses) &&
                   PhoneNumber == model.PhoneNumber &&
                   FullName == model.FullName &&
                   Location == model.Location;
        }

        private bool AreAddressesEqual(ObservableCollection<AddressModel> addresses)
        {
            if (Addresses.Count != addresses.Count)
            {
                return false;
            }
            for (int i = 0; i < Addresses.Count; i++)
            {
                if (!Addresses[i].Equals(addresses[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
