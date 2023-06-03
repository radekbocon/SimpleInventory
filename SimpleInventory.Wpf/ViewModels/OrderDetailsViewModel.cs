using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SimpleInventory.Core.Extentions;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.Controls;
using SimpleInventory.Wpf.Controls.Dialogs;
using SimpleInventory.Wpf.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace SimpleInventory.Wpf.ViewModels
{
    public class OrderDetailsViewModel : ViewModelBase
    {
        private readonly ICustomerService _customerService;
        private readonly INavigationService _navigationService;
        private readonly IOrderService _orderService;
        private readonly IInventoryService _inventoryService;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        private OrderViewModel? _order;
        private OrderViewModel? _orderBackup;
        private ICommand? _goBackCommand;
        private ICommand? _editCustomerCommand;
        private ICommand? _editBillingAddressCommand;
        private ICommand? _editDeliveryAddressrCommand;
        private ICommand? _editLineCommand;
        private ICommand? _addLineCommand;
        private ICommand? _updateTotalCommand;
        private ICommand? _saveOrderCommand;


        public string CustomerButtonText => Order?.Customer == null ? "Add" : "Edit";
        public string DeliveryAddressButtonText => Order?.DeliveryAddress == null ? "Add" : "Edit";
        public string BillingAddressButtonText => Order?.BillingAddress == null ? "Add" : "Edit";

        public ICommand SaveOrderCommand
        {
            get
            {
                if (_saveOrderCommand == null)
                {
                    _saveOrderCommand = new RelayCommand(
                        async p => await SaveOrder(),
                        p => Order?.Lines != null);
                }

                return _saveOrderCommand;
            }
        }

        public ICommand UpdateTotalCommand
        {
            get
            {
                if (_updateTotalCommand == null)
                {
                    _updateTotalCommand = new RelayCommand(
                        p => CalculateTotal(),
                        p => true);
                }

                return _updateTotalCommand;
            }
        }

        public ICommand AddLineCommand
        {
            get
            {
                if (_addLineCommand == null)
                {
                    _addLineCommand = new RelayCommand(
                        p => AddLine(),
                        p => true);
                }

                return _addLineCommand;
            }
        }

        public ICommand EditLineCommand
        {
            get
            {
                if (_editLineCommand == null)
                {
                    _editLineCommand = new RelayCommand(
                        p => EditLine((OrderLineViewModel)p),
                        p => p is OrderLineViewModel);
                }

                return _editLineCommand;
            }
        }

        public ICommand EditCustomerCommand
        {
            get
            {
                if (_editCustomerCommand == null)
                {
                    _editCustomerCommand = new RelayCommand(
                        p => EditCustomer(),
                        p => true);
                }

                return _editCustomerCommand;
            }
        }

        public ICommand EditBillingAddressCommand
        {
            get
            {
                if (_editBillingAddressCommand == null)
                {
                    _editBillingAddressCommand = new RelayCommand(
                        p => EditBillingAddress(),
                        p => Order?.Customer != null);
                }

                return _editBillingAddressCommand;
            }
        }

        public ICommand EditDeliveryAddressCommand
        {
            get
            {
                if (_editDeliveryAddressrCommand == null)
                {
                    _editDeliveryAddressrCommand = new RelayCommand(
                        p => EditDeliveryAddress(),
                        p => Order?.Customer != null);
                }

                return _editDeliveryAddressrCommand;
            }
        }

        public OrderViewModel Order
        {
            get
            {
                return _order;
            }
            set 
            {
                SetProperty(ref _order, value);
                NotifyPropertyChanged(nameof(CustomerButtonText));
                NotifyPropertyChanged(nameof(DeliveryAddressButtonText));
                NotifyPropertyChanged(nameof(BillingAddressButtonText));
            }
        }

        public ICommand GoBackCommand
        {
            get
            {
                if (_goBackCommand == null)
                {
                    _goBackCommand = new RelayCommand(
                        p => GoBack(),
                        p => true);
                }

                return _goBackCommand;
            }
        }

        public OrderDetailsViewModel(ICustomerService customerService, IOrderService orderService, INavigationService navigationService, IInventoryService inventoryService, IMapper mapper, INotificationService notificationService)
        {
            _customerService = customerService;
            _orderService = orderService;
            _navigationService = navigationService;
            _inventoryService = inventoryService;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public OrderDetailsViewModel Initialize(string id = null)
        {
            SetOrder(id).Await();
            return this;
        }

        private void GoBack()
        {
            _navigationService.GoBack();
        }

        private async Task SetOrder(string id)
        {
            if (id == null)
            {
                Order = new OrderViewModel();
            }
            else
            {
                var model = await _orderService.GetByNumberAsync(id);
                Order = _mapper.Map<OrderViewModel>(model);
                _orderBackup = new OrderViewModel(Order);
            }
        }

        private void EditCustomer()
        {
            var vm = new PickCustomerViewModel(_customerService, _navigationService, _mapper, customer =>
            {
                Order.Customer = customer;
                NotifyPropertyChanged(nameof(Order));
                NotifyPropertyChanged(nameof(CustomerButtonText));
            });

            _navigationService.ShowDialog(vm, callback: result => { });
        }

        private void EditBillingAddress()
        {
            var vm = new PickAddressViewModel(Order?.Customer?.Id, _customerService, _navigationService, _mapper, address =>
            {
                Order.BillingAddress = address;
                NotifyPropertyChanged(nameof(Order));
                NotifyPropertyChanged(nameof(CustomerButtonText));
            });

            _navigationService.ShowDialog(vm, callback: result => { });
        }

        private void EditDeliveryAddress()
        {
            var vm = new PickAddressViewModel(Order?.Customer?.Id, _customerService, _navigationService, _mapper, address =>
            {
                Order.DeliveryAddress = address;
                NotifyPropertyChanged(nameof(Order));
                NotifyPropertyChanged(nameof(CustomerButtonText));
            });

            _navigationService.ShowDialog(vm, callback: result => { });
        }

        private void EditLine(OrderLineViewModel line)
        {
            var vm = new PickProductItemViewModel(_inventoryService, _navigationService, _mapper, inventory =>
            {
                line.Item = inventory.Item;
                line.Price = line.Item.Price;
                NotifyPropertyChanged(nameof(Order));
                NotifyPropertyChanged(nameof(Order.Lines));
            });

            _navigationService.ShowDialog(vm, callback: result => { });
        }

        private void AddLine()
        {
            Order.Lines = Order.Lines ?? new ObservableCollection<OrderLineViewModel>();

            var vm = new PickProductItemViewModel(_inventoryService, _navigationService, _mapper, inventory =>
            {
                var line = new OrderLineViewModel(inventory.Item);
                Order.Lines.Add(line);
                line.Number = Order.Lines.IndexOf(line) + 1;
                line.Price = line.Item.Price;
                NotifyPropertyChanged(nameof(Order));
                NotifyPropertyChanged(nameof(Order.Lines));
            });

            _navigationService.ShowDialog(vm, callback: result => { });
        }

        private void CalculateTotal()
        {
            if (Order?.Lines != null)
            {
                decimal total = 0;
                foreach (var item in Order.Lines)
                {
                    total += item.Total;
                }
                Order.OrderTotal = total;
            }
        }

        private async Task SaveOrder()
        {
            try
            {
                var model = _mapper.Map<OrderModel>(Order);
                await _orderService.UpsertOneAsync(model);
                _notificationService.Show("Saved", "Order succesfully saved.", NotificationType.Info);
            }
            catch (Exception ex)
            {
                _notificationService.Show("Error", "Not enough items in inventory.", NotificationType.Error, 6);
            }
        }
    }
}
