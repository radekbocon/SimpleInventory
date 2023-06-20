using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimpleInventory.Core.Models
{
    public class OrderLineModel
    {
        public int Number { get; set; }
        public bool IsCancelled { get; set; } = false;
        public ItemModel? Item { get; set; }
        public uint Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public string? PickLocation { get; set; }
    }
}