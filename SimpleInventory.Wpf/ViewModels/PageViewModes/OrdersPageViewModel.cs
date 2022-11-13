using AutoMapper;
using Bogus;
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
using System.Windows.Controls;
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
        private readonly IMapper _mapper;

        private ObservableCollection<OrderSummaryViewModel> _orders;
        private ObservableCollection<OrderSummaryViewModel> _filteredOrders;
        private ICommand _loadOrdersCommand;
        private ICommand _addNewOrderCommand;
        private ICommand _openOrderCommand;
        private string _searchTerxt;
        private bool _isBusy;
        private double _scrollPosition;

        public OrdersPageViewModel(ICustomerService customerService, IOrderService orderService, INavigationService navigationService, IInventoryService inventoryService, IMapper mapper)
        {
            _customerService = customerService;
            _orderService = orderService;
            _navigationService = navigationService;
            _inventoryService = inventoryService;
            _mapper = mapper;
            //GenerateFakeOrders();
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
                        async p => await AddNewOrder(),
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
                        async p => await OpenOrder((OrderSummaryViewModel)p),
                        p => p is OrderSummaryViewModel);
                }

                return _openOrderCommand;
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
            var vmList = _mapper.Map<ObservableCollection<OrderSummaryViewModel>>(list);
            Orders = new ObservableCollection<OrderSummaryViewModel>(vmList);
            IsBusy = false;
        }

        private async Task AddNewOrder()
        {
            bool save = false;
            var vm = new OrderDetailsViewModel(_navigationService, _orderService, _customerService, _inventoryService, _mapper);
            _navigationService.OpenPage(vm);

            if (save)
            {
                await GetOrders();
            }
        }

        private async Task OpenOrder(OrderSummaryViewModel order)
        {
            if (order.Id == null) return;

            bool save = false;
            var vm = new OrderDetailsViewModel(order.Id, _navigationService, _orderService, _customerService, _inventoryService, _mapper);
            _navigationService.OpenPage(vm);

            if (save)
            {
                await GetOrders();
            }
        }

        private void ShowBusyIndicator()
        {
            if (Orders == null)
            {
                IsBusy = true;
            }
        }

        //private void GenerateFakeOrders()
        //{
        //    var address = new Faker<AddressModel>()
        //        .RuleFor(x => x.Line1, f => f.Address.StreetAddress())
        //        .RuleFor(x => x.Line2, f => f.Address.SecondaryAddress())
        //        .RuleFor(x => x.PhoneNumber, f => f.Person.Phone)
        //        .RuleFor(x => x.City, f => f.Address.City())
        //        .RuleFor(x => x.Country, f => f.Address.CountryCode())
        //        .RuleFor(x => x.PostCode, f => f.Address.ZipCode());

        //    var customer = new Faker<CustomerModel>()
        //        .RuleFor(x => x.FirstName, f => f.Person.FirstName)
        //        .RuleFor(x => x.LastName, f => f.Person.LastName)
        //        .RuleFor(x => x.CompanyName, f => f.Company.CompanyName() + " " + f.Company.CompanySuffix())
        //        .RuleFor(x => x.Email, f => f.Person.Email)
        //        .RuleFor(x => x.PhoneNumber, f => f.Person.Phone)
        //        .RuleFor(x => x.Addresses, f => address.Generate(2));

        //    var item = new Faker<ItemModel>()
        //        .RuleFor(x => x.ProductId, f => f.Random.AlphaNumeric(5).ToUpper())
        //        .RuleFor(x => x.Name, f => f.Commerce.ProductName())
        //        .RuleFor(x => x.Type, f => f.Commerce.Product())
        //        .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
        //        .RuleFor(x => x.Price, f => decimal.Parse(f.Commerce.Price()))
        //        .RuleFor(x => x.Quantity, f => f.Random.Int(0, 450));

        //    var orderLine = new Faker<OrderLine>()
        //        .RuleFor(x => x.IsCancelled, f => f.Random.Bool(0.8f))
        //        .RuleFor(x => x.Item, f => item.Generate())
        //        .RuleFor(x => x.Price, f => f.Random.Decimal(0, 3000))
        //        .RuleFor(x => x.Quantity, f => f.Random.Int(0, 5000));

            //var order = new Faker<OrderModel>()
            //    .RuleFor(x => x.Customer, f => customer.Generate())
            //    .RuleFor(x => x.StartDate, f => f.Date.Recent())
            //    .RuleFor(x => x.LastUpdateDate, f => f.Date.Recent())
            //    .RuleFor(x => x.CloseDate, f => f.Date.Recent())
            //    .RuleFor(x => x.OrderTotal, f => f.Random.Decimal(1, 100000))
            //    .RuleFor(x => x.BillingAddress, f => address.Generate())
            //    .RuleFor(x => x.BillingAddress, f => address.Generate())
            //    .RuleFor(x => x.Lines, f => orderLine.Generate(5))
            //    .RuleFor(x => x.Status, f => f.Random.Enum<OrderStatus>());


            //var list = order.Generate(400);

            //_orderService.UpsertMany(list);

        //}
    }
}
