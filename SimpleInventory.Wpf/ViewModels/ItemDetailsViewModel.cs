using SimpleInventory.Core.Extentions;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.Dialogs;
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
        private ItemModel? _item;
        private ICommand? _saveCommand;
        private ICommand? _cancelCommand;
        private readonly IInventoryService? _inventoryService;
        private readonly IDialogService? _dialogService;

        public ItemDetailsViewModel(string itemId, IInventoryService inventoryService, IDialogService dialogService)
        {
            _inventoryService = inventoryService;
            _dialogService = dialogService;
            SetItem(itemId).Await();
        }


        public ItemDetailsViewModel(IInventoryService inventoryService, IDialogService dialogService)
        {
            _inventoryService = inventoryService;
            _dialogService = dialogService;
        }

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
                        p => true);
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
                        p => p is ItemModel);
                }

                return _saveCommand;
            }
        }

        private void Cancel()
        {
            _dialogService.DialogResult(false);
        }

        private async Task Save()
        {
            await _inventoryService.UpsertOne(Item);
            _dialogService.DialogResult(true);
        }

        private async Task SetItem(string id)
        {
            Item = await _inventoryService.GetById(id);
        }
    }
}
