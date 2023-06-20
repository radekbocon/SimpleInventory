using AutoMapper;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.Controls;
using SimpleInventory.Wpf.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleInventory.Wpf.ViewModels
{
    public class MoveInventoryViewModel : ViewModelBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly INavigationService _navigationService;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private ICommand? _saveCommand;
        private ICommand? _cancelCommand;
        private InventoryEntryViewModel _entry;
        private InventoryEntryViewModel _entryBackup;
        private uint _quantityToMove;
        private string _newLocation;
        private Action _onItemReceived;

        public string Name { get; set; } = "Move";

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
                        p => CanSave());
                }

                return _saveCommand;
            }
        }

        public uint QuantityToMove
        {
            get => _quantityToMove;
            set => SetProperty(ref _quantityToMove, value);
        }

        public string NewLocation 
        { 
            get => _newLocation; 
            set => SetProperty(ref _newLocation, value );
        }

        public InventoryEntryViewModel Entry
        {
            get => _entry;
            set => SetProperty(ref _entry, value);
        }

        public MoveInventoryViewModel(IInventoryService inventoryService, INavigationService navigationService, IMapper mapper, INotificationService notificationService)
        {
            _inventoryService = inventoryService;
            _navigationService = navigationService;
            _mapper = mapper;
            _notificationService = notificationService;

        }

        public MoveInventoryViewModel Initialize(string id, Action onItemReceived)
        {
            InitializeEntry(id);
            _onItemReceived = onItemReceived;
            return this;
        }

        private void InitializeEntry(string id)
        {
            var entry = _inventoryService.GetById(id);
            Entry = _mapper.Map<InventoryEntryViewModel>(entry);
            _entryBackup = new InventoryEntryViewModel(Entry);
        }

        private void Cancel()
        {
            Entry = _entryBackup;
        }

        private async Task Save()
        {
            var model = _mapper.Map<InventoryEntryModel>(Entry);
            await _inventoryService.MoveItemAsync(model, NewLocation, QuantityToMove);
            _entryBackup = Entry;
            _onItemReceived.Invoke();
            _navigationService.CloseModal();
            ShowSavedNotification(model);
        }

        private void ShowSavedNotification(InventoryEntryModel model)
        {
            string message = $"Moved {model.Quantity} of {model?.Item?.Name} to {NewLocation}";
            _notificationService.Show("Moved", message, NotificationType.Info);
        }

        private bool CanSave()
        {
            return Entry.Quantity >= QuantityToMove && QuantityToMove > 0 && !string.IsNullOrWhiteSpace(NewLocation);
        }


    }
}
