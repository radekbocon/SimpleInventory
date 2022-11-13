using MongoDB.Bson.Serialization.Attributes;
using SimpleInventory.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Wpf.ViewModels
{
    public class OrderViewModel : ViewModelBase
    {
        private ObservableCollection<OrderLineViewModel> _lines = new ObservableCollection<OrderLineViewModel>();
        private decimal _orderTotal;
        private CustomerViewModel? _customer;

        public string? Id { get; set; }
        public CustomerViewModel? Customer 
        { 
            get => _customer; 
            set => SetProperty(ref _customer, value); 
        }
        public DateTime StartDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public DateTime CloseDate { get; set; }
        public decimal OrderTotal
        {
            get => _orderTotal;
            set => SetProperty(ref _orderTotal, value);

        }
        public AddressModel? BillingAddress { get; set; }
        public AddressModel? DeliveryAddress { get; set; }
        public ObservableCollection<OrderLineViewModel> Lines
        {
            get
            {
                return _lines;
            }
            set
            {
                SetProperty(ref _lines, value);
                NotifyPropertyChanged(nameof(OrderTotal));
            }
        }
        public OrderStatus Status { get; set; }

        public OrderViewModel()
        {

        }

        public OrderViewModel(OrderViewModel order)
        {
            Id = order.Id;
            Customer = order.Customer;
            StartDate = order.StartDate;
            LastUpdateDate = order.LastUpdateDate;
            CloseDate = order.CloseDate;
            OrderTotal = order.OrderTotal;
            BillingAddress = order.BillingAddress;
            DeliveryAddress = order.DeliveryAddress;
            Lines = new ObservableCollection<OrderLineViewModel>(order.Lines);
        }
    }
}
