using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Core.Models
{
    public class InventoryEntryModel : INotifyPropertyChanged
    {
        private string? _location;
        private ItemModel? _item;
        private uint _quantity;

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Location 
        { 
            get => _location; 
            set
            {
                _location = value;
                RaisePropertyChanged();
            }
        }
        public ItemModel? Item 
        { 
            get => _item;
            set
            {
                _item = value;
                RaisePropertyChanged();
            }
        }

        public uint Quantity 
        { 
            get => _quantity;
            set
            {
                _quantity = value;
                RaisePropertyChanged();
            }
        }

        public InventoryEntryModel()
        {

        }

        public InventoryEntryModel(InventoryEntryModel entry)
        {
            Id = entry.Id;
            Location = entry.Location;
            Item = new ItemModel(entry.Item);
            Quantity = entry.Quantity;
        }

        public override bool Equals(object? obj)
        {
            return obj is InventoryEntryModel model &&
                   Id == model.Id &&
                   Location == model.Location &&
                   Item != null &&
                   Item.Equals(model.Item) &&
                   Quantity == model.Quantity;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
