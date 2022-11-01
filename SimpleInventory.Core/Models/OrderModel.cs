using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Core.Models
{
    public class OrderModel
    {
        private List<OrderLine>? _lines;

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        public CustomerModel? Customer { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public DateTime CloseDate { get; set; }
        public decimal OrderTotal { get; set; }
        public AddressModel? BillingAddress { get; set; }
        public AddressModel? DeliveryAddress { get; set; }
        public List<OrderLine>? Lines 
        { 
            get => _lines; 
            set
            {
                _lines = value;

                if (_lines != null)
                {
                    foreach (var line in _lines)
                    {
                        line.Number = line.Number == 0 ? (_lines.IndexOf(line) + 1) : line.Number;
                    }
                }
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
    }
}
