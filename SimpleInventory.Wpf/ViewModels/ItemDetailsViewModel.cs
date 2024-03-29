﻿using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SimpleInventory.Core.Extentions;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.Controls;
using SimpleInventory.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleInventory.Wpf.ViewModels
{
    public class ItemDetailsViewModel : ViewModelBase
    {
        private ItemViewModel? _item;
        private ItemViewModel? _itemBackup;
        private ICommand? _saveCommand;
        private ICommand? _cancelCommand;
        private readonly IInventoryService _inventoryService;
        private readonly INavigationService _navigationService;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public ItemDetailsViewModel(IInventoryService inventoryService, INavigationService navigationService, IMapper mapper, INotificationService notificationService)
        {
            _inventoryService = inventoryService;
            _navigationService = navigationService;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public ItemDetailsViewModel Create(string id = null)
        {
            Initialize(id).Await();
            return this;
        }

        public string Name { get; set; } = "Item Details";

        public ItemViewModel Item
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
                        p => HasItemChanged());
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
                        p => HasItemChanged() && !Item.HasErrors);
                }

                return _saveCommand;
            }
        }

        private void Cancel()
        {
            Item = new ItemViewModel(_itemBackup);
        }

        private async Task Save()
        {
            Item.Validate();
            if (Item.HasErrors)
            {
                return;
            }
            var model = _mapper.Map<ItemModel>(Item);
            await _inventoryService.UpsertOneItemAsync(model);
            _itemBackup = new ItemViewModel(Item);
            _navigationService.CloseModal();
            _notificationService.Show("Saved", "Item succesfully saved.", NotificationType.Info);
        }

        private async Task Initialize(string id)
        {
            Item = id == null ? new ItemViewModel() : _mapper.Map<ItemViewModel>(await _inventoryService.GetItemByIdAsync(id));
            _itemBackup = new ItemViewModel(Item); 
        }

        private bool HasItemChanged()
        {
            if (_item == null || _itemBackup == null)
            {
                return false;
            }
            bool hasChanged = !_item.Equals(_itemBackup);
            return hasChanged;
        }
    }
}
