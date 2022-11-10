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
    [BsonIgnoreExtraElements]
    public class ItemModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public decimal Price { get; set; }

        public ItemModel()
        {

        }

        public ItemModel(ItemModel item)
        {
            if (item == null)
            {
                return;
            }
            Id = item.Id;
            ProductId = item.ProductId;
            Name = item.Name;
            Description = item.Description;
            Type = item.Type;
            Price = item.Price;
        }

        public override bool Equals(object? obj)
        {
            return obj is ItemModel model &&
                   Id == model.Id &&
                   ProductId == model.ProductId &&
                   Name == model.Name &&
                   Description == model.Description &&
                   Type == model.Type &&
                   Price == model.Price;
        }
    }
}
