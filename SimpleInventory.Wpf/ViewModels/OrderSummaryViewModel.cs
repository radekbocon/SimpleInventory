using SimpleInventory.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Wpf.ViewModels
{
    public class OrderSummaryViewModel : ViewModelBase
    {
        private CustomerViewModel? _customer;
        private DateTime _lastUpdateDate;
        private decimal _orderTotal;
        private OrderStatus _status;

        public string? Id { get; set; }
        public CustomerViewModel? Customer
        {
            get => _customer;
            set => SetProperty(ref _customer, value);
        }
        public DateTime LastUpdateDate
        {
            get => _lastUpdateDate;
            set => SetProperty(ref _lastUpdateDate, value);
        }
        public decimal OrderTotal
        {
            get => _orderTotal;
            set => SetProperty(ref _orderTotal, value);
        }
        public OrderStatus Status 
        { 
            get => _status; 
            set => SetProperty(ref _status, value);
        }
    }
}
