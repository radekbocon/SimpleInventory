﻿using Bogus;
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
using System.Windows.Input;

namespace SimpleInventory.Wpf.ViewModels.PageViewModes
{
    public class OrdersPageViewModel : PageViewModel
    {
        public string Name { get; set; } = "Orders";

        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly INavigationService _dialogService;

        private ObservableCollection<OrderSummaryModel> _orders;
        private ObservableCollection<OrderSummaryModel> _filteredOrders;
        private ICommand _loadOrdersCommand;
        private ICommand _addNewOrderCommand;
        private ICommand _openOrderCommand;
        private string _searchTerxt;
        private bool _isBusy;

        public OrdersPageViewModel(ICustomerService customerService, IOrderService orderService, INavigationService dialogService)
        {
            _customerService = customerService;
            _orderService = orderService;
            _dialogService = dialogService;
            //GenerateFakeOrders();
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

        public ObservableCollection<OrderSummaryModel> Orders
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SearchText) || SearchText == string.Empty)
                {
                    return _orders;
                }

                IEnumerable<OrderSummaryModel> list = FilterOrders();
                _filteredOrders = new ObservableCollection<OrderSummaryModel>(list);
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
                        async p => await OpenCustomer((OrderSummaryModel)p),
                        p => p is OrderSummaryModel);
                }

                return _openOrderCommand;
            }
        }

        private IEnumerable<OrderSummaryModel> FilterOrders()
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
            Orders = new ObservableCollection<OrderSummaryModel>(list);
            IsBusy = false;
        }

        private async Task AddNewOrder()
        {
            //bool save = false;
            //var vm = new CustomerDetailsViewModel(_customerService, _dialogService);
            //_dialogService.ShowDialog(vm, result =>
            //{
            //    save = result;
            //});

            //if (save)
            //{
            //    await GetOrders();
            //}
            throw new NotImplementedException();
        }

        private async Task OpenCustomer(OrderSummaryModel order)
        {
            //if (customer.Id == null) return;

            //bool save = false;
            //var vm = new CustomerDetailsViewModel(customer.Id, _customerService, _dialogService);
            //_dialogService.ShowDialog(vm, result =>
            //{
            //    save = result;
            //});

            //if (save)
            //{
            //    await GetOrders();
            //}
            throw new NotImplementedException();
        }

        private void ShowBusyIndicator()
        {
            if (Orders == null)
            {
                IsBusy = true;
            }
        }

        private void GenerateFakeOrders()
        {
            var address = new Faker<AddressModel>()
                .RuleFor(x => x.Line1, f => f.Address.StreetAddress())
                .RuleFor(x => x.Line2, f => f.Address.SecondaryAddress())
                .RuleFor(x => x.PhoneNumber, f => f.Person.Phone)
                .RuleFor(x => x.City, f => f.Address.City())
                .RuleFor(x => x.Country, f => f.Address.CountryCode())
                .RuleFor(x => x.PostCode, f => f.Address.ZipCode());

            var customer = new Faker<CustomerModel>()
                .RuleFor(x => x.FirstName, f => f.Person.FirstName)
                .RuleFor(x => x.LastName, f => f.Person.LastName)
                .RuleFor(x => x.CompanyName, f => f.Company.CompanyName() + " " + f.Company.CompanySuffix())
                .RuleFor(x => x.Email, f => f.Person.Email)
                .RuleFor(x => x.PhoneNumber, f => f.Person.Phone)
                .RuleFor(x => x.Addresses, f => address.Generate(2));

            var item = new Faker<ItemModel>()
                .RuleFor(x => x.ProductId, f => f.Random.AlphaNumeric(5).ToUpper())
                .RuleFor(x => x.Name, f => f.Commerce.ProductName())
                .RuleFor(x => x.Type, f => f.Commerce.Product())
                .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
                .RuleFor(x => x.Price, f => double.Parse(f.Commerce.Price()))
                .RuleFor(x => x.Quantity, f => f.Random.Int(0, 450));

            var orderLine = new Faker<OrderLine>()
                .RuleFor(x => x.IsCancelled, f => f.Random.Bool(0.8f))
                .RuleFor(x => x.Item, f => item.Generate())
                .RuleFor(x => x.Price, f => f.Random.Decimal(0, 3000))
                .RuleFor(x => x.Quantity, f => f.Random.Int(0, 5000));

            var order = new Faker<OrderModel>()
                .RuleFor(x => x.Customer, f => customer.Generate())
                .RuleFor(x => x.StartDate, f => f.Date.Recent())
                .RuleFor(x => x.LastUpdateDate, f => f.Date.Recent())
                .RuleFor(x => x.CloseDate, f => f.Date.Recent())
                .RuleFor(x => x.OrderTotal, f => f.Random.Decimal(1, 100000))
                .RuleFor(x => x.BillingAddress, f => address.Generate())
                .RuleFor(x => x.BillingAddress, f => address.Generate())
                .RuleFor(x => x.Lines, f => orderLine.Generate(5))
                .RuleFor(x => x.Status, f => f.Random.Enum<OrderStatus>());


            var list = order.Generate(400);

            _orderService.UpsertMany(list);

        }
    }
}
