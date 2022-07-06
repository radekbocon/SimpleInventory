using SimpleInventory.Core.Extentions;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace SimpleInventory.Wpf.ViewModels.PageViewModes
{
    public class InventoryPageViewModel : PageViewModel
    {
        private readonly IInventoryService _inventyoryService;
        private readonly IDialogService _dialogService;

        private bool _isFilteredResult = false;
        private string _searchText;
        private ObservableCollection<ItemModel> _filteredInventory;
        private ObservableCollection<ItemModel> _inventory;
        private ICommand _addToDraftCommand;
        private ICommand _deleteItemCommand;
        private ICommand _saveChangesCommand;
        private ICommand _loadItemsCommand;

        public string Name { get; set; } = "Inventory";

        public InventoryPageViewModel(IInventoryService inventyoryService, IDialogService dialogService)
        {
            _inventyoryService = inventyoryService;
            _dialogService = dialogService;
        }

        public string SearchText
        {
            get => _searchText; 
            set 
            { 
                SetProperty(ref _searchText, value);
                RaisePropertyChanged(nameof(Inventory));
            }
        }

        public ObservableCollection<ItemModel> Inventory
        {
            get
            {
                if (SearchText == null || SearchText == string.Empty)
                {
                    _isFilteredResult = false;
                    return _inventory;
                }

                IEnumerable<ItemModel> list = FilterInventory();
                _filteredInventory = new ObservableCollection<ItemModel>(list);
                _isFilteredResult = true;
                return _filteredInventory;
            }
            set 
            {
                SetProperty(ref _inventory, value); 
            }
        }

        public ICommand AddToDraftCommand
        {
            get
            {
                if (_addToDraftCommand == null)
                {
                    _addToDraftCommand = new RelayCommand(
                        p => SetItemAsDraft((ItemModel)p), 
                        p => p is ItemModel);
                }

                return _addToDraftCommand;
            }
        }

        public ICommand DeleteItemCommand
        {
            get
            {
                if (_deleteItemCommand == null)
                {
                    _deleteItemCommand = new RelayCommand(
                        async p => await DeleteItem((ItemModel)p),
                        p => p is ItemModel);
                }

                return _deleteItemCommand;
            }
        }

        public ICommand SaveChangesCommand
        {
            get 
            {
                if (_saveChangesCommand == null)
                {
                    _saveChangesCommand = new RelayCommand(
                        async p => await UpsertItems(),
                        p => p is ItemModel);
                }

                return _saveChangesCommand;
            }
        }

        public ICommand LoadItemsCommand
        {
            get
            {
                if (_loadItemsCommand == null)
                {
                    _loadItemsCommand = new RelayCommand(
                        async p => await GetItems(),
                        p => p is ItemModel);
                }

                return _loadItemsCommand;
            }
        }

        private IEnumerable<ItemModel> FilterInventory()
        {
            return from i in _inventory
                   where i.Name != null && i.Name.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
                         i.ProductId != null && i.ProductId.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
                         i.Description != null && i.Description.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
                         i.Type != null && i.Type.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase)
                   select i;
        }

        private void SetItemAsDraft(ItemModel item)
        {
            item.IsDraft = true;
            if (item.Id != null)
            {
                _inventory.Where(x => x.Id == item.Id).Single().IsDraft = true;
            }
            if (_isFilteredResult)
            {
                _filteredInventory.Remove(item);
                _inventory.Add(item);
            }
            _inventory = new ObservableCollection<ItemModel>(_inventory);
            RaisePropertyChanged(nameof(Inventory));
        }

        private async Task DeleteItem(ItemModel item)
        {
            bool delete = false;
            _dialogService.ShowDialog("YesNoDialogView", result => 
            {
                delete = result;
            });

            if (delete)
            {
                await _inventyoryService.DeleteInventoryItem(item)
                    .ContinueWith(async t => await GetItems());
            }
        }

        private async Task UpsertItems()
        {
            var list = new List<ItemModel>(Inventory.Where(x => x.IsDraft == true));
            await _inventyoryService.UpsertInventoryItems(list)
                .ContinueWith(async t => await GetItems());
        }

        private async Task GetItems()
        {
            await Task.Delay(4000);
            var list = await _inventyoryService.GetInventyoryItems();
            Inventory = new ObservableCollection<ItemModel>(list);
        }

    }
}
