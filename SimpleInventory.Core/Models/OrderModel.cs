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
    public class OrderModel : INotifyPropertyChanged
    {
        private ObservableCollection<OrderLine>? _lines;
        private decimal _orderTotal;

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        public CustomerModel? Customer { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public DateTime CloseDate { get; set; }
        public decimal OrderTotal 
        {
            get
            {
                return _orderTotal;
            }
            set
            {
                _orderTotal = value;
                RaisePropertyChanged();
            }
            
        }
        public AddressModel? BillingAddress { get; set; }
        public AddressModel? DeliveryAddress { get; set; }
        public ObservableCollection<OrderLine>? Lines 
        { 
            get => _lines; 
            set
            {
                _lines = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(OrderTotal));
            } 
        }
        public OrderStatus Status { get; set; }

        public OrderModel()
        {

        }

        public OrderModel(OrderModel order)
        {
            Id = order.Id;
            Customer = order.Customer;
            StartDate = order.StartDate;
            LastUpdateDate = order.LastUpdateDate;
            CloseDate = order.CloseDate;
            OrderTotal = order.OrderTotal;
            BillingAddress = order.BillingAddress;
            DeliveryAddress = order.DeliveryAddress;
            Lines = order.Lines;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
