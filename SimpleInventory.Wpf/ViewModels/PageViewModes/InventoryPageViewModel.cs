using Bogus;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.Controls.Dialogs;
using SimpleInventory.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleInventory.Wpf.ViewModels.PageViewModes
{
    public class InventoryPageViewModel : PageViewModel
    {
        private readonly IInventoryService _inventyoryService;
        private readonly Services.INavigationService _dialogService;

        private string _searchText;
        private ObservableCollection<ItemModel> _filteredInventory;
        private ObservableCollection<ItemModel> _inventory;
        private ICommand _deleteItemCommand;
        private ICommand _addNewItemCommand;
        private ICommand _loadItemsCommand;
        private ICommand _editItemCommand;
        private bool _isBusy;

        public string Name { get; set; } = "Inventory";

        public InventoryPageViewModel(IInventoryService inventyoryService, INavigationService dialogService)
        {
            _inventyoryService = inventyoryService;
            _dialogService = dialogService;
            //GenerateFakeItems();
        }

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
                NotifyPropertyChanged(nameof(Inventory));
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
                        p => true);
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
                        p => true);
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
            var vm = new ItemDetailsViewModel(_inventyoryService, _dialogService);
            _dialogService.ShowModal(vm, result =>
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
            if (item.Id == null) return;

            bool save = false;
            var vm = new ItemDetailsViewModel(item.Id, _inventyoryService, _dialogService);
            _dialogService.ShowModal(vm, result =>
            {
                save = result;
            });

            if (save)
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
                await _inventyoryService.DeleteOne(item)
                    .ContinueWith(async t => await GetItems());
            }
        }

        private async Task GetItems()
        {
            ShowBusyIndicator();
            var list = await _inventyoryService.GetAll();
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

        //private void GenerateFakeItems()
        //{
        //    var item = new Faker<ItemModel>()
        //        .RuleFor(x => x.ProductId, f => f.Random.AlphaNumeric(5).ToUpper())
        //        .RuleFor(x => x.Name, f => f.Commerce.ProductName())
        //        .RuleFor(x => x.Type, f => f.Commerce.Product())
        //        .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
        //        .RuleFor(x => x.Price, f => decimal.Parse(f.Commerce.Price()))
        //        .RuleFor(x => x.Quantity, f => f.Random.Int(0, 450));

        //    var list = item.Generate(550);

        //    _inventyoryService.UpsertMany(list);

        //}

    }
}
