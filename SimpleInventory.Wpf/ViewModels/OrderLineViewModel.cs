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
        private ItemViewModel? _item;
        private uint _quantity;
        private decimal _price;
        private decimal _total;
        private bool _isCancelled = false;
        private int _number;
        private string _pickLocation;

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
            get => _item;
            set
            {
                SetProperty(ref _item, value);
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
        public string PickLocation
        {
            get => _pickLocation;
            set => SetProperty(ref _pickLocation, value);
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
