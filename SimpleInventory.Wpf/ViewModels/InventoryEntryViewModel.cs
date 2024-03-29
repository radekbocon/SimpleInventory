﻿using MongoDB.Bson.Serialization.Attributes;
using SimpleInventory.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Wpf.ViewModels
{
    public class InventoryEntryViewModel : ViewModelBase
    {
        private string? _location;
        private ItemViewModel? _item;
        private uint _quantity;
        private uint _lockedQuantity;

        public string? Id { get; set; }
        public string? Location
        {
            get => _location;
            set
            {
                SetProperty(ref _location, value);
            }
        }
        public ItemViewModel? Item
        {
            get => _item;
            set
            {
                SetProperty(ref _item, value);
            }
        }

        public uint Quantity
        {
            get => _quantity;
            set
            {
                SetProperty(ref _quantity, value);
            }
        }
        public string DisplayProperty => $"{Item?.Name} (In Inventory: {Quantity})";

        public uint LockedQuantity 
        { 
            get => _lockedQuantity; 
            set => SetProperty(ref _lockedQuantity, value);
        }

        public InventoryEntryViewModel()
        {

        }

        public InventoryEntryViewModel(InventoryEntryViewModel entry)
        {
            Id = entry.Id;
            Location = entry.Location;
            Item = new ItemViewModel(entry.Item);
            Quantity = entry.Quantity;
        }

        public override bool Equals(object? obj)
        {
            return obj is InventoryEntryViewModel model &&
                   Id == model.Id &&
                   Location == model.Location &&
                   EqualityComparer<ItemViewModel?>.Default.Equals(Item, model.Item) &&
                   Quantity == model.Quantity &&
                   DisplayProperty == model.DisplayProperty;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Location, Item, Quantity, DisplayProperty);
        }
    }
}
