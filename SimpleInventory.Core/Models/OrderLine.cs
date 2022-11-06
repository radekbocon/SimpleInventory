using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimpleInventory.Core.Models
{
    public class OrderLine : INotifyPropertyChanged
    {
        private ItemModel? item;
        private uint quantity;
        private decimal price;
        private decimal total;


        public int Number { get; set; }
        public bool IsCancelled { get; set; } = false;
        public ItemModel? Item
        {
            get => item;
            set
            {
                item = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Total));
            }
        }
        public uint Quantity
        {
            get => quantity;
            set
            {
                quantity = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Total));
            }
        }
        public decimal Price 
        { 
            get => price;
            set
            {
                price = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Total));
            }
        }
        public decimal Total
        {
            get => Price * Quantity;
            set
            {
                total = value;
                RaisePropertyChanged();
            }
        }

        public OrderLine(ItemModel? item)
        {
            Item = item;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}