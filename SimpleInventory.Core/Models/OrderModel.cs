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
    public class OrderModel
    {
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
        public List<OrderLineModel>? Lines { get; set; } = new List<OrderLineModel>();
        public OrderStatus Status { get; set; }
    }
}
