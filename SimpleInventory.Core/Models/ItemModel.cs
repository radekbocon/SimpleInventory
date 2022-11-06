using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Core.Models
{
    public class ItemModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public uint Quantity { get; set; }
        public decimal Price { get; set; }

        public ItemModel()
        {

        }

        public ItemModel(ItemModel item)
        {
            Id = item.Id;
            ProductId = item.ProductId;
            Name = item.Name;
            Description = item.Description;
            Type = item.Type;
            Quantity = item.Quantity;
            Price = item.Price;
        }
    }
}
