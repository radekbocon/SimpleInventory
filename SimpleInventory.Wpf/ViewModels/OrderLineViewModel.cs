using SimpleInventory.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Wpf.ViewModels
{
    public class OrderLineViewModel : ViewModelBase
    {
        private ItemViewModel? item;
        private uint _quantity;
        private decimal _price;
        private decimal _total;
        private bool _isCancelled = false;
        private int _number;

        public int Number 
        { 
            get => _number; 
            set => SetProperty(ref _number, value);
        }
        public bool IsCancelled
        {
            get => _isCancelled;
            set => SetProperty(ref _isCancelled, value);
        }
        public ItemViewModel? Item
        {
            get => item;
            set
            {
                SetProperty(ref item, value);
                NotifyPropertyChanged(nameof(Total));
            }
        }
        public uint Quantity
        {
            get => _quantity;
            set
            {
                SetProperty(ref _quantity, value);
                NotifyPropertyChanged(nameof(Total));
            }
        }
        public decimal Price
        {
            get => _price;
            set
            {
                SetProperty(ref _price, value);
                NotifyPropertyChanged(nameof(Total));
            }
        }
        public decimal Total
        {
            get => Price * Quantity;
            set
            {
                _total = value;
            }
        }

        public OrderLineViewModel()
        {

        }

        public OrderLineViewModel(ItemViewModel? item)
        {
            Item = item;
        }
    }
}
