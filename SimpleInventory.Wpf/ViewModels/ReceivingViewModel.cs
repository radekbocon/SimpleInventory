using SimpleInventory.Core.Extentions;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleInventory.Wpf.ViewModels
{
    public class ReceivingViewModel : ViewModelBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly INavigationService _navigationService;
        private ICommand? _saveCommand;
        private ICommand? _cancelCommand;
        private ICommand? _selectionChangedCommand;
        private InventoryEntryModel _entry;
        private ObservableCollection<ItemModel> _items;
        private ItemModel _selectedItem;

        public string DisplayProperty => nameof(SelectedItem.Name);

        public string Name { get; set; } = "Receiving";

        public ICommand SelectionChangedCommand
        {
            get
            {
                if (_selectionChangedCommand == null)
                {
                    _selectionChangedCommand = new RelayCommand(
                        p => ChangeSelectedItem(),
                        p => true);
                }

                return _selectionChangedCommand;
            }
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
                        p => Entry != null);
                }

                return _saveCommand;
            }
        }

        public ItemModel SelectedItem 
        { 
            get => _selectedItem; 
            set => SetProperty(ref _selectedItem, value); 
        }

        public ObservableCollection<ItemModel> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }


        public InventoryEntryModel Entry
        {
            get => _entry;
            set => SetProperty(ref _entry, value);
        }

        public ReceivingViewModel(IInventoryService inventoryService, INavigationService navigationService)
        {
            _inventoryService = inventoryService;
            _navigationService = navigationService;
            GetItems();
            Entry = new InventoryEntryModel();
        }

        public ReceivingViewModel(string id, IInventoryService inventoryService, INavigationService navigationService)
        {
            _inventoryService = inventoryService;
            _navigationService = navigationService;
            GetItems();
            Entry = _inventoryService.GetById(id);
            Entry.Quantity = 0;
            SelectedItem = Items?.Where(x => x.Id == Entry?.Item?.Id).FirstOrDefault();
        }

        private void GetItems()
        {
            var list = _inventoryService.GetAllItems();
            Items = new ObservableCollection<ItemModel>(list);
        }

        private void Cancel()
        {
            _navigationService.ModalResult(false);
        }

        private async Task Save()
        {
            await _inventoryService.ReceiveItem(Entry);
            _navigationService.ModalResult(true);
        }

        private void ChangeSelectedItem()
        {
            Entry.Item = SelectedItem;
        }
    }
}
