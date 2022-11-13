using AutoMapper;
using SimpleInventory.Core.Extentions;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.Services;
using SimpleInventory.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleInventory.Wpf.Controls.Dialogs
{
    public abstract class PickItemViewModel<T> : ViewModelBase where T : class
    {
        protected readonly INavigationService _navigationService;
        protected readonly IMapper _mapper;
        private ObservableCollection<T> _items;
        private Action<T> _action;
        private T _selectedListItem;
        private ICommand _cancelCommand;
        private ICommand _pickItemCommand;
        private ICommand _loadItemsCommand;

        public abstract string Title { get; }
        public abstract string DisplayProperty { get; }

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

        public ICommand PickItemCommand
        {
            get
            {
                if (_pickItemCommand == null)
                {
                    _pickItemCommand = new RelayCommand(
                        p => PickItem(),
                        p => SelectedItem != null);
                }

                return _pickItemCommand;
            }
        }

        public T SelectedItem
        {
            get => _selectedListItem;
            set
            {
                SetProperty(ref _selectedListItem, value);
            }
        }

        public ObservableCollection<T> Items
        {
            get => _items;
            set
            {
                SetProperty(ref _items, value);
            }
        }

        public PickItemViewModel(INavigationService navigationService, IMapper mapper, Action<T> callback)
        {
            _navigationService = navigationService;
            _action = callback;
            _mapper = mapper;
        }

        protected abstract Task GetItems();

        private void PickItem()
        {
            _action.Invoke(SelectedItem);
            _navigationService.DialogResult(true);
        }

        private void Cancel()
        {
            _navigationService.DialogResult(false);
        }
    }
}
