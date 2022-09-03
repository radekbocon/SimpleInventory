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

        private string _searchText;
        private ObservableCollection<ItemModel> _filteredInventory;
        private ObservableCollection<ItemModel> _inventory;
        private ICommand _deleteItemCommand;
        private ICommand _addNewItemCommand;
        private ICommand _loadItemsCommand;
        private ICommand _editItemCommand;

        public string Name { get; set; } = "Inventory";

        public InventoryPageViewModel(IInventoryService inventyoryService, IDialogService dialogService)
        {
            _inventyoryService = inventyoryService;
            _dialogService = dialogService;
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set { SetProperty(ref _isBusy, value); }
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
                if (string.IsNullOrWhiteSpace(SearchText) || SearchText == string.Empty)
                {
                    return _inventory;
                }

                IEnumerable<ItemModel> list = FilterInventory();
                _filteredInventory = new ObservableCollection<ItemModel>(list);
                return _filteredInventory;
            }
            set 
            {
                SetProperty(ref _inventory, value); 
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

        public ICommand AddNewItemCommand
        {
            get 
            {
                if (_addNewItemCommand == null)
                {
                    _addNewItemCommand = new RelayCommand(
                        async p => await AddNewItem(),
                        p => p is ItemModel);
                }

                return _addNewItemCommand;
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

        public ICommand EditItemCommand
        {
            get
            {
                if (_editItemCommand == null)
                {
                    _editItemCommand = new RelayCommand(
                        async p => await EditItem((ItemModel)p),
                        p => p is ItemModel);
                }

                return _editItemCommand;
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

        private async Task AddNewItem()
        {
            bool save = false;
            var vm = new EditItemViewModel(new ItemModel(), _inventyoryService, _dialogService);
            _dialogService.ShowDialog(vm, result =>
            {
                save = result;
            });

            if (save)
            {
                await GetItems();
            }
        }

        private async Task EditItem(ItemModel item)
        {
            if (item == null) return;

            bool save = false;
            var vm = new EditItemViewModel(item, _inventyoryService, _dialogService);
            _dialogService.ShowDialog(vm, result =>
            {
                save = result;
            });

            if (!save)
            {
                await GetItems();
            }
        }

        private async Task DeleteItem(ItemModel item)
        {
            if (item == null) return;

            bool delete = false;
            var vm = new YesNoDialogViewModel(_dialogService, "Delete this record?");
            _dialogService.ShowDialog(viewModel: vm, dialogWidth: 250, callback: result => 
            {
                delete = result;
            });

            if (delete)
            {
                await _inventyoryService.DeleteInventoryItem(item)
                    .ContinueWith(async t => await GetItems());
            }
        }

        private async Task GetItems()
        {
            ShowBusyIndicator();
            var list = await _inventyoryService.GetInventyoryItems();
            Inventory = new ObservableCollection<ItemModel>(list);
            IsBusy = false;
        }

        private void ShowBusyIndicator()
        {
            if (Inventory == null)
            {
                IsBusy = true;
            }
        }

    }
}
