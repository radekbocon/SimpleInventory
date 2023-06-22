using AutoMapper;
using SimpleInventory.Core.Exceptions;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.Controls;
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
    public class OrdersPageViewModel : PageViewModel
    {
        public string Name { get; set; } = "Orders";

        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly INavigationService _navigationService;
        private readonly IInventoryService _inventoryService;
        private readonly INotificationService _notificationService;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IMapper _mapper;

        private ObservableCollection<OrderSummaryViewModel> _orders;
        private ObservableCollection<OrderSummaryViewModel> _filteredOrders;
        private ICommand _loadOrdersCommand;
        private ICommand _addNewOrderCommand;
        private ICommand _openOrderCommand;
        private ICommand _receiveOrderCommand;
        private string _searchTerxt;
        private bool _isBusy;
        private double _scrollPosition;

        public OrdersPageViewModel(ICustomerService customerService, IOrderService orderService, INavigationService navigationService, IInventoryService inventoryService, IMapper mapper, INotificationService notificationService, IViewModelFactory viewModelFactory)
        {
            _customerService = customerService;
            _orderService = orderService;
            _navigationService = navigationService;
            _inventoryService = inventoryService;
            _mapper = mapper;
            _notificationService = notificationService;
            _viewModelFactory = viewModelFactory;
        }

        public double ScrollPosition 
        { 
            get => _scrollPosition; 
            set => SetProperty(ref _scrollPosition, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set { SetProperty(ref _isBusy, value); }
        }


        public string SearchText
        {
            get => _searchTerxt;
            set
            {
                SetProperty(ref _searchTerxt, value);
                NotifyPropertyChanged(nameof(Orders));
            }
        }

        public ObservableCollection<OrderSummaryViewModel> Orders
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SearchText) || SearchText == string.Empty)
                {
                    return _orders;
                }

                IEnumerable<OrderSummaryViewModel> list = FilterOrders();
                _filteredOrders = new ObservableCollection<OrderSummaryViewModel>(list);
                return _filteredOrders;
            }
            set
            {
                SetProperty(ref _orders, value);
            }
        }

        public ICommand LoadOrdersCommand
        {
            get
            {
                if (_loadOrdersCommand == null)
                {
                    _loadOrdersCommand = new RelayCommand(
                        async p => await GetOrders(),
                        p => true);
                }

                return _loadOrdersCommand;
            }
        }

        public ICommand AddNewOrderCommand
        {
            get
            {
                if (_addNewOrderCommand == null)
                {
                    _addNewOrderCommand = new RelayCommand(
                        p => AddNewOrder(),
                        p => true);
                }

                return _addNewOrderCommand;
            }
        }

        public ICommand OpenOrderCommand
        {
            get
            {
                if (_openOrderCommand == null)
                {
                    _openOrderCommand = new RelayCommand(
                        p => OpenOrder((OrderSummaryViewModel)p),
                        p => p is OrderSummaryViewModel);
                }

                return _openOrderCommand;
            }
        }

        public ICommand ReceiveOrderCommand
        {
            get
            {
                if (_receiveOrderCommand == null)
                {
                    _receiveOrderCommand = new RelayCommand(
                        async p => await ReceiveOrder((OrderSummaryViewModel)p),
                        p => p is OrderSummaryViewModel vm && CanReceiveOrder(vm));
                }

                return _receiveOrderCommand;
            }
        }

        private IEnumerable<OrderSummaryViewModel> FilterOrders()
        {
            return from i in _orders
                   where i.Customer?.CompanyName != null && i.Customer.CompanyName.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
                         i.Customer?.FullName != null && i.Customer.FullName.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
                         i.Status.ToString().Contains(SearchText, StringComparison.CurrentCultureIgnoreCase)
                   select i;
        }

        private async Task GetOrders()
        {
            ShowBusyIndicator();
            var list = await _orderService.GetAll();
            var vmList = _mapper.Map<List<OrderSummaryViewModel>>(list);
            vmList = vmList.OrderByDescending(x => x.LastUpdateDate).ToList();
            Orders = new ObservableCollection<OrderSummaryViewModel>(vmList);
            IsBusy = false;
        }

        private void AddNewOrder()
        {
            var vm = _viewModelFactory.Create<OrderDetailsViewModel>().Initialize();
            _navigationService.OpenPage(vm);
        }

        private void OpenOrder(OrderSummaryViewModel order)
        {
            if (order?.Id == null) return;

            var vm = _viewModelFactory.Create<OrderDetailsViewModel>().Initialize(order.Id);
            _navigationService.OpenPage(vm);
        }

        private async Task ReceiveOrder(OrderSummaryViewModel order)
        {
            if (order?.Id == null) return;

            try
            {
                var orderModel = _orderService.GetById(order.Id);
                orderModel.Status = OrderStatus.Received;
                await _orderService.UpsertOneAsync(orderModel);
                await GetOrders();
                _notificationService.Show("Received", "Order succesfully received.", NotificationType.Info);
            }
            catch (InvalidTransactionException ex)
            {
                _notificationService.Show("Error", ex.Message, NotificationType.Error, 6);
            }

        }

        private bool CanReceiveOrder(OrderSummaryViewModel order)
        {
            return order.Status == OrderStatus.Draft;
        }

        private void ShowBusyIndicator()
        {
            if (Orders == null)
            {
                IsBusy = true;
            }
        }
    }
}
