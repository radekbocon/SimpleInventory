﻿using AutoMapper;
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
        private readonly INavigationService _navigationService;
        private readonly IMapper _mapper;

        private string _searchText;
        private ObservableCollection<InventoryEntryViewModel> _filteredInventory;
        private ObservableCollection<InventoryEntryViewModel> _inventory;
        private ICommand _deleteItemCommand;
        private ICommand _addNewItemCommand;
        private ICommand _loadInventoryCommand;
        private ICommand _editEntryCommand;
        private ICommand _receiveCommand;
        private bool _isBusy;

        public string Name { get; set; } = "Inventory";

        public InventoryPageViewModel(IInventoryService inventyoryService, INavigationService dialogService, IMapper mapper)
        {
            _inventyoryService = inventyoryService;
            _navigationService = dialogService;
            _mapper = mapper;
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
                        p => p is InventoryEntryModel);
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

        public ICommand EditEntryCommand
        {
            get
            {
                if (_editEntryCommand == null)
                {
                    _editEntryCommand = new RelayCommand(
                        async p => await EditEntry((InventoryEntryViewModel)p),
                        p => p is InventoryEntryViewModel);
                }

                return _editEntryCommand;
            }
        }

        public ICommand ReceiveCommand
        {
            get
            {
                if (_receiveCommand == null)
                {
                    _receiveCommand = new RelayCommand(
                        async p => await ReceiveItem(),
                        p => true);
                }

                return _receiveCommand;
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

        private async Task AddNewItem()
        {
            bool save = false;
            var vm = new ItemDetailsViewModel(_inventyoryService, _navigationService);
            _navigationService.ShowModal(vm, result =>
            {
                save = result;
            });

            if (save)
            {
                await GetInventory();
            }
        }

        private async Task EditEntry(InventoryEntryViewModel entry)
        {
            if (entry.Id == null) return;

            bool save = false;
            var vm = new ReceivingViewModel(entry.Id, _inventyoryService, _navigationService, _mapper);
            _navigationService.ShowModal(vm, result =>
            {
                save = result;
            });

            if (save)
            {
                await GetInventory();
            }
        }

        private async Task DeleteItem(InventoryEntryViewModel item)
        {
            if (item == null) return;

            var model = _mapper.Map<InventoryEntryModel>(item);
            bool delete = false;
            var vm = new YesNoDialogViewModel(_navigationService, "Delete this record?");
            _navigationService.ShowDialog(viewModel: vm, dialogWidth: 250, callback: result => 
            {
                delete = result;
            });

            if (delete)
            {
                await _inventyoryService.DeleteOne(model)
                    .ContinueWith(async t => await GetInventory());
            }
        }

        private async Task GetInventory()
        {
            ShowBusyIndicator();
            var list = await _inventyoryService.GetAllEntriesAsync();
            var vmList = _mapper.Map<List<InventoryEntryViewModel>>(list);
            Inventory = new ObservableCollection<InventoryEntryViewModel>(vmList);
            IsBusy = false;
        }

        private async Task ReceiveItem()
        {
            bool save = false;
            var vm = new ReceivingViewModel(_inventyoryService, _navigationService, _mapper);
            _navigationService.ShowModal(vm, result =>
            {
                save = result;
            });

            if (save)
            {
                await GetInventory();
            }
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
