using SimpleInventory.Core.Extentions;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleInventory.Wpf.ViewModels
{
    public class ItemDetailsViewModel : ViewModelBase
    {
        private ItemModel _item;
        private ItemModel _itemBackup;
        private ICommand? _saveCommand;
        private ICommand? _cancelCommand;
        private readonly IInventoryService _inventoryService;
        private readonly INavigationService _dialogService;

        public ItemDetailsViewModel(string itemId, IInventoryService inventoryService, INavigationService dialogService)
        {
            _inventoryService = inventoryService;
            _dialogService = dialogService;
            SetItem(itemId).Await();
        }

        public ItemDetailsViewModel(IInventoryService inventoryService, INavigationService dialogService)
        {
            _inventoryService = inventoryService;
            _dialogService = dialogService;
            SetItem().Await();
        }

        public string Name { get; set; } = "Item Details";

        public ItemModel Item
        {
            get { return _item; }
            set { SetProperty(ref _item, value); }
        }

        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(
                        p => Cancel(),
                        p => HasItemChanged());
                }

                return _cancelCommand;
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(
                        async p => await Save(),
                        p => HasItemChanged());
                }

                return _saveCommand;
            }
        }

        private void Cancel()
        {
            Item = new ItemModel(_itemBackup);
        }

        private async Task Save()
        {
            await _inventoryService.UpsertOneItemAsync(Item);
            await SetItem(Item.Id);
            _dialogService.ModalResult(true);
        }

        private async Task SetItem(string id = null)
        {
            Item = id == null ? new ItemModel() : await _inventoryService.GetItemByIdAsync(id);
            _itemBackup = new ItemModel(Item);
        }

        private bool HasItemChanged()
        {
            if (_item == null || _itemBackup == null)
            {
                return false;
            }
            bool hasChanged = !_item.Equals(_itemBackup);
            return hasChanged;
        }
    }
}
