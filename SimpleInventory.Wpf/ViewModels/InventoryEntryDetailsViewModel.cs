using AutoMapper;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.Controls;
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
    public class InventoryEntryDetailsViewModel : ViewModelBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly INavigationService _navigationService;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private ICommand? _saveCommand;
        private ICommand? _cancelCommand;
        private ICommand? _selectionChangedCommand;
        private InventoryEntryViewModel _entry;
        private InventoryEntryViewModel _entryBackup;
        private ObservableCollection<ItemViewModel> _items;
        private ItemViewModel _selectedItem;

        private Action _onSaved;

        public string DisplayProperty => nameof(SelectedItem.DisplayProperty);

        public string Name { get; set; } = "Correction";

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
                        p => HasEntryChanged());
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
                        p => HasEntryChanged());
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

        public InventoryEntryDetailsViewModel(IInventoryService inventoryService, INavigationService navigationService, IMapper mapper, INotificationService notificationService)
        {
            _inventoryService = inventoryService;
            _navigationService = navigationService;
            _mapper = mapper;
            _notificationService = notificationService;

        }

        public InventoryEntryDetailsViewModel Initialize(string id, Action onSaved)
        {
            GetItems();
            InitializeEntry(id);
            _onSaved = onSaved;
            return this;
        }

        private void GetItems()
        {
            var list = _inventoryService.GetAllItems();
            var vmList = _mapper.Map<List<ItemViewModel>>(list);
            Items = new ObservableCollection<ItemViewModel>(vmList);
        }

        private void Cancel()
        {
            Entry = _entryBackup;
            SelectedItem = Entry.Item ?? null;
        }

        private async Task Save()
        {
            var model = _mapper.Map<InventoryEntryModel>(Entry);
            if (model.Quantity == 0)
            {
                await _inventoryService.DeleteOne(model);
            }
            else
            {
                await _inventoryService.UpsertOneAsync(model);
            }
            _onSaved.Invoke();
            _navigationService.CloseModal();
            ShowSavedNotification(model);
        }

        private void ShowSavedNotification(InventoryEntryModel model)
        {
            string message = "Changes saved.";
            _notificationService.Show("Saved", message, NotificationType.Info);
        }

        private void ChangeSelectedItem()
        {
            Entry.Item = SelectedItem;
        }

        private bool HasEntryChanged()
        {
            return !Entry.Equals(_entryBackup);
        }

        private void InitializeEntry(string id)
        {
            var entry = _inventoryService.GetById(id);
            Entry = _mapper.Map<InventoryEntryViewModel>(entry);
            SelectedItem = Items?.Where(x => x.Id == Entry?.Item?.Id).FirstOrDefault();
            _entryBackup = new InventoryEntryViewModel(Entry);
            

        }
    }
}
