﻿using AutoMapper;
using Bogus;
using Microsoft.Extensions.DependencyInjection;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.Controls;
using SimpleInventory.Wpf.Controls.Dialogs;
using SimpleInventory.Wpf.Factories;
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
        private readonly IInventoryService _inventoryService;
        private readonly INavigationService _navigationService;
        private readonly INotificationService _notificationService;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IMapper _mapper;

        private string _searchText;
        private ObservableCollection<InventoryEntryViewModel> _filteredInventory;
        private ObservableCollection<InventoryEntryViewModel> _inventory;
        private ICommand _deleteItemCommand;
        private ICommand _addNewItemCommand;
        private ICommand _loadInventoryCommand;
        private ICommand _receiveItemCommand;
        private ICommand _receiveNewItemCommand;
        private ICommand _moveItemCommand;
        private bool _isBusy;

        public string Name { get; set; } = "Inventory";

        public InventoryPageViewModel(IInventoryService inventyoryService, INavigationService dialogService, IMapper mapper, INotificationService notificationService, IViewModelFactory viewModelFactory)
        {
            _inventoryService = inventyoryService;
            _navigationService = dialogService;
            _mapper = mapper;
            _notificationService = notificationService;
            _viewModelFactory = viewModelFactory;
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

        public ObservableCollection<InventoryEntryViewModel> Inventory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SearchText) || SearchText == string.Empty)
                {
                    return _inventory;
                }

                IEnumerable<InventoryEntryViewModel> list = FilterInventory();
                _filteredInventory = new ObservableCollection<InventoryEntryViewModel>(list);
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
                        async p => await DeleteItem((InventoryEntryViewModel)p),
                        p => p is InventoryEntryViewModel);
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
                        p =>  AddNewItem(),
                        p => true);
                }

                return _addNewItemCommand;
            }
        }

        public ICommand LoadInventoryCommand
        {
            get
            {
                if (_loadInventoryCommand == null)
                {
                    _loadInventoryCommand = new RelayCommand(
                        async p => await GetInventory(),
                        p => true);
                }

                return _loadInventoryCommand;
            }
        }

        public ICommand ReceiveItemCommand
        {
            get
            {
                if (_receiveItemCommand == null)
                {
                    _receiveItemCommand = new RelayCommand(
                        p => ReceiveItem((InventoryEntryViewModel)p),
                        p => p is InventoryEntryViewModel);
                }

                return _receiveItemCommand;
            }
        }

        public ICommand ReceiveNewItemCommand
        {
            get
            {
                if (_receiveNewItemCommand == null)
                {
                    _receiveNewItemCommand = new RelayCommand(
                        p => ReceiveItem(),
                        p => true);
                }

                return _receiveNewItemCommand;
            }
        }

        public ICommand MoveItemCommand
        {
            get
            {
                if (_moveItemCommand == null)
                {
                    _moveItemCommand = new RelayCommand(
                        p => MoveItem((InventoryEntryViewModel)p),
                        p => p is InventoryEntryViewModel);
                }

                return _moveItemCommand;
            }
        }

        private ICommand _editItemCommand;

        public ICommand EditItemCommand
        {
            get
            {
                if (_editItemCommand == null)
                {
                    _editItemCommand = new RelayCommand(
                        p => EditItem((InventoryEntryViewModel)p),
                        p => p is InventoryEntryViewModel);
                }

                return _editItemCommand;
            }
        }

        private IEnumerable<InventoryEntryViewModel> FilterInventory()
        {
            return from i in _inventory
                   where i.Item?.Name != null && i.Item.Name.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
                         i.Item?.ProductId != null && i.Item.ProductId.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
                         i.Item?.Description != null && i.Item.Description.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
                         i.Location != null && i.Location.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
                         i.Item?.Type != null && i.Item.Type.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase)
                   select i;
        }

        private async void AddNewItem()
        {
            var vm = _viewModelFactory.Create<ItemDetailsViewModel>();
            _navigationService.ShowModal(vm);
        }

        private void ReceiveItem(InventoryEntryViewModel entry)
        {
            if (entry?.Id == null) return;

            var vm = _viewModelFactory.
                Create<ReceivingViewModel>()
                .Initialize(entry.Id, async () => await GetInventory());
            _navigationService.ShowModal(vm);
        }

        private void ReceiveItem()
        {
            var vm = _viewModelFactory
                .Create<ReceivingViewModel>()
                .Initialize(async () => await GetInventory());
            _navigationService.ShowModal(vm);
        }

        private void MoveItem(InventoryEntryViewModel entry)
        {
            if (entry?.Id is null) return;

            var vm = _viewModelFactory
                .Create<MoveInventoryViewModel>()
                .Initialize(entry.Id, async () => await GetInventory());
            _navigationService.ShowModal(vm);
        }

        private void EditItem(InventoryEntryViewModel entry)
        {
            if (entry?.Id is null) return;

            var vm = _viewModelFactory
            .Create<InventoryEntryDetailsViewModel>()
            .Initialize(entry.Id, async () => await GetInventory());
            _navigationService.ShowModal(vm);
        }

        private async Task DeleteItem(InventoryEntryViewModel item)
        {
            if (item == null) return;

            var model = _mapper.Map<InventoryEntryModel>(item);
            bool delete = false;
            var vm = new YesNoDialogViewModel("Delete this record?");
            _navigationService.ShowDialog(viewModel: vm, dialogWidth: 250, callback: result => 
            {
                delete = result;
            });

            if (delete)
            {
                await _inventoryService.DeleteOne(model)
                    .ContinueWith(async t => await GetInventory());
            }
        }

        private async Task GetInventory()
        {
            ShowBusyIndicator();
            var list = await _inventoryService.GetAllEntriesAsync();
            var vmList = _mapper.Map<List<InventoryEntryViewModel>>(list);
            Inventory = new ObservableCollection<InventoryEntryViewModel>(vmList);
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
