using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Core.Models
{
    public class CustomerModel : INotifyPropertyChanged
    {
        private List<AddressModel> _addresses = new();

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? CompanyName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public List<AddressModel> Addresses
        {
            get => _addresses; 
            set
            {
                _addresses = value;
            }
        }
        [BsonIgnore]
        public string? FullName => $"{FirstName} {LastName}";
        [BsonIgnore]
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

        public CustomerModel(CustomerModel customer)
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

        public CustomerModel()
        {

        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
