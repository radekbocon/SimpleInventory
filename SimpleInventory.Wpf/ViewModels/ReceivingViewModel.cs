using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SimpleInventory.Core.Extentions;
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
    public class ReceivingViewModel : ViewModelBase
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

        private Action _onItemReceived;

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
                        p => CanSave() && HasEntryChanged());
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

        public ReceivingViewModel(Action onItemReceived)
        {
            _inventoryService = App.Current.Services.GetRequiredService<IInventoryService>();
            _navigationService = App.Current.Services.GetRequiredService<INavigationService>();
            _notificationService = App.Current.Services.GetRequiredService<INotificationService>();
            _mapper = App.Current.Services.GetRequiredService<IMapper>();
            GetItems();
            Entry = new InventoryEntryViewModel();
            _entryBackup = new InventoryEntryViewModel();
            _onItemReceived = onItemReceived;
        }

        public ReceivingViewModel(string id, Action onItemReceived)
        {
            _inventoryService = App.Current.Services.GetRequiredService<IInventoryService>();
            _navigationService = App.Current.Services.GetRequiredService<INavigationService>();
            _notificationService = App.Current.Services.GetRequiredService<INotificationService>();
            _mapper = App.Current.Services.GetRequiredService<IMapper>();
            GetItems();
            Initialaze(id);
            _onItemReceived = onItemReceived;
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
            await _inventoryService.ReceiveItem(model);
            _entryBackup = Entry;
            _onItemReceived.Invoke();
            _navigationService.CloseModal();
            ShowSavedNotification(model);
        }

        private void ShowSavedNotification(InventoryEntryModel model)
        {
            string message = model.Quantity > 1 ? $"Received {model.Quantity} {model?.Item?.Name}'s" : $"Received {model.Quantity} {model?.Item?.Name}";
            _notificationService.Show("Received", message, NotificationType.Info);
        }

        private void ChangeSelectedItem()
        {
            Entry.Item = SelectedItem;
        }

        private bool CanSave()
        {
            return Entry != null && 
                Entry.Quantity > 0 && 
                Entry.Item != null && 
                Entry.Location != null;
        }

        private bool HasEntryChanged()
        {
            return !Entry.Equals(_entryBackup);
        }

        private void Initialaze(string id)
        {
            var entry = _inventoryService.GetById(id);
            Entry = _mapper.Map<InventoryEntryViewModel>(entry);
            Entry.Quantity = 0;
            SelectedItem = Items?.Where(x => x.Id == Entry?.Item?.Id).FirstOrDefault();
            _entryBackup = new InventoryEntryViewModel(Entry);
        }
    }
}
