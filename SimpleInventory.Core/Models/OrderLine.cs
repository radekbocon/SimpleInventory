namespace SimpleInventory.Core.Models
{
    public class OrderLine
    {
        public bool IsCancelled { get; set; } = false;
        public ItemModel? Item { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}