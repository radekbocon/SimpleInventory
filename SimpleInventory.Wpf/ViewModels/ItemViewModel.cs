using SimpleInventory.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

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
        public string? ProductId
        {
            get => _productId;
            set
            {
                SetProperty(ref _productId, value);
                ValidateProductId();
            }
        }
        public string? Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
                ValidateName();
            }
        }
        public string? Description
        {
            get => _description;
            set
            {
                SetProperty(ref _description, value);
                ValidateDescription();
            }
        }
        public string? Type
        {
            get => _type;
            set
            {
                SetProperty(ref _type, value);
                ValidateType();
            }
        }
        public decimal Price
        {
            get => _price;
            set
            {
                SetProperty(ref _price, value);
                ValidatePrice();
            }
        }
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

        public void Validate()
        {
            ValidateProductId();
            ValidateName();
            ValidateDescription();
            ValidateType();
            ValidatePrice();
        }

        private void ValidateProductId()
        {
            ClearErrors(nameof(ProductId));
            if (string.IsNullOrEmpty(ProductId))
            {
                AddError(nameof(ProductId), "Product Id is required.");
            }
            if (ProductId?.Length > 50)
            {
                AddError(nameof(ProductId), "Product Id can be at most 50 characters long.");
            }
        }

        private void ValidateName()
        {
            ClearErrors(nameof(Name));
            if (string.IsNullOrEmpty(Name))
            {
                AddError(nameof(Name), "Name is required.");
            }
            if (Name?.Length > 50)
            {
                AddError(nameof(Name), "Name can be at most 50 characters long.");
            }
        }

        private void ValidateDescription()
        {
            ClearErrors(nameof(Description));
            if (string.IsNullOrEmpty(Description))
            {
                AddError(nameof(Description), "Description is required.");
            }
            if (Description?.Length > 200)
            {
                AddError(nameof(Description), "Description can be at most 200 characters long.");
            }
        } 

        private void ValidateType()
        {
            ClearErrors(nameof(Type));
            if (string.IsNullOrEmpty(Type))
            {
                AddError(nameof(Type), "Type is required.");
            }
            if (Type?.Length > 50)
            {
                AddError(nameof(Type), "Type can be at most 50 characters long.");
            }
        }

        private void ValidatePrice()
        {
            ClearErrors(nameof(Price));
            if (Price < 0)
            {
                AddError(nameof(Name), "Price should not be negative.");
            }
        }
    }
}
