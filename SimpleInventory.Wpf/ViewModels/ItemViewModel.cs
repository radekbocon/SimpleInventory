using MongoDB.Bson.Serialization.Attributes;
using SimpleInventory.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Wpf.ViewModels
{
    public class ItemViewModel : ViewModelBase
    {
        private string? _productId;
        private string? _name;
        private string? _description;
        private string? _type;
        private decimal _price;

        public string? Id { get; set; }
        public string? ProductId { get => _productId; set => SetProperty(ref _productId, value); }
        public string? Name { get => _name; set => SetProperty(ref _name, value); }
        public string? Description { get => _description; set =>SetProperty(ref _description, value); }
        public string? Type { get => _type; set => SetProperty(ref _type, value); }
        public decimal Price { get => _price; set => SetProperty(ref _price, value); }
        public string DisplayProperty => $"{ProductId} | {Name}";

        public ItemViewModel()
        {

        }

        public ItemViewModel(ItemViewModel item)
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
            return obj is ItemViewModel model &&
                   Id == model.Id &&
                   ProductId == model.ProductId &&
                   Name == model.Name &&
                   Description == model.Description &&
                   Type == model.Type &&
                   Price == model.Price;
        }
    }
}
