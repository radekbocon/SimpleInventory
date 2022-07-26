using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleInventory.Wpf.Dialogs
{
    public class EditItemViewModel : ViewModelBase
    {
        public int Width { get; set; } = 400;
        public int Height { get; set; } = 100;
        private ItemModel _item;
        private ICommand _saveCommand;
        private IInventoryService _inventoryService;

        public EditItemViewModel(ItemModel item, IInventoryService inventoryService)
        {
            Item = item;
            _inventoryService = inventoryService;
        }

        public ItemModel Item
        {
            get { return _item; }
            set { SetProperty(ref _item, value); }
        }

        public ICommand SaveCommand 
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(
                        async p => await Save(),
                        p => p is ItemModel);
                }

                return _saveCommand;
            }
        }

        private async Task Save()
        {
            await _inventoryService.UpsertInventoryItem(Item);
        }
    }
}
