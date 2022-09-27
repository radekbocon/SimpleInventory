using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Core.Models
{
    [BsonIgnoreExtraElements]
    public class OrderSummaryModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        public CustomerModel? Customer { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public decimal OrderTotal { get; set; }
        public OrderStatus Status { get; set; }
    }
}
