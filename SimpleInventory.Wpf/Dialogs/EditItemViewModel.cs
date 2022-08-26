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
        private ItemModel _item;
        private ICommand _saveCommand;
        private ICommand _cancelCommand;
        private readonly IInventoryService _inventoryService;
        private readonly IDialogService _dialogService;

        public EditItemViewModel(ItemModel item, IInventoryService inventoryService, IDialogService dialogService)
        {
            Item = item;
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
            await _inventoryService.UpsertInventoryItem(Item);
            _dialogService.DialogResult(true);
        }
    }
}
