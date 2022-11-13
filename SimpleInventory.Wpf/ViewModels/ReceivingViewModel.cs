using AutoMapper;
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
        private readonly IMapper _mapper;
        private ICommand? _saveCommand;
        private ICommand? _cancelCommand;
        private ICommand? _selectionChangedCommand;
        private InventoryEntryViewModel _entry;
        private ObservableCollection<ItemViewModel> _items;
        private ItemViewModel _selectedItem;

        public string DisplayProperty => nameof(SelectedItem.DisplayProperty);

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

        public ItemViewModel SelectedItem 
        { 
            get => _selectedItem; 
            set => SetProperty(ref _selectedItem, value); 
        }

        public ObservableCollection<ItemViewModel> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }


        public InventoryEntryViewModel Entry
        {
            get => _entry;
            set => SetProperty(ref _entry, value);
        }

        public ReceivingViewModel(IInventoryService inventoryService, INavigationService navigationService, IMapper mapper)
        {
            _inventoryService = inventoryService;
            _navigationService = navigationService;
            _mapper = mapper;
            GetItems();
            Entry = new InventoryEntryViewModel();
        }

        public ReceivingViewModel(string id, IInventoryService inventoryService, INavigationService navigationService, IMapper mapper)
        {
            _inventoryService = inventoryService;
            _navigationService = navigationService;
            _mapper = mapper;
            GetItems();
            var entry = _inventoryService.GetById(id);
            Entry = _mapper.Map<InventoryEntryViewModel>(entry);
            Entry.Quantity = 0;
            SelectedItem = Items?.Where(x => x.Id == Entry?.Item?.Id).FirstOrDefault();
        }

        private void GetItems()
        {
            var list = _inventoryService.GetAllItems();
            var vmList = _mapper.Map<List<ItemViewModel>>(list);
            Items = new ObservableCollection<ItemViewModel>(vmList);
        }

        private void Cancel()
        {
            _navigationService.ModalResult(false);
        }

        private async Task Save()
        {
            var model = _mapper.Map<InventoryEntryModel>(Entry);
            await _inventoryService.ReceiveItem(model);
            _navigationService.ModalResult(true);
        }

        private void ChangeSelectedItem()
        {
            Entry.Item = SelectedItem;
        }
    }
}
