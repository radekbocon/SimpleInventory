namespace SimpleInventory.Core.Models
{
    public class OrderLine
    {
        public int Number { get; set; }
        public bool IsCancelled { get; set; } = false;
        public ItemModel? Item { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total  => Quantity * Price;
    }
}