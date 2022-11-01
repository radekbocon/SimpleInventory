using SimpleInventory.Core.Extentions;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.Controls.Dialogs;
using SimpleInventory.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleInventory.Wpf.ViewModels
{
    public class OrderDetailsViewModel : ViewModelBase
    {
        private readonly ICustomerService _customerService;
        private readonly INavigationService _navigationService;
        private readonly IOrderService _orderService;

		private OrderModel? _order;
        private OrderModel? _orderBackup;
        private ICommand _goBackCommand;
        private ICommand _editCustomerCommand;
        private ICommand _editBillingAddressCommand;
        private ICommand _editDeliveryAddressrCommand;



        public string CustomerButtonText => Order?.Customer == null ? "Add" : "Edit";
        public string DeliveryAddressButtonText => Order?.DeliveryAddress == null ? "Add" : "Edit";
        public string BillingAddressButtonText => Order?.BillingAddress == null ? "Add" : "Edit";

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
                        p => EditCustomer(),
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
                        p => EditCustomer(),
                        p => Order?.Customer != null);
                }

                return _editDeliveryAddressrCommand;
            }
        }

        public OrderModel Order
        {
            get => _order;
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

        public OrderDetailsViewModel(INavigationService navigationService, IOrderService orderService, ICustomerService customerService)
        {
            _navigationService = navigationService;
            _customerService = customerService;
            _orderService = orderService;
            Order = new OrderModel();
        }

        public OrderDetailsViewModel(string id, INavigationService navigationService, IOrderService orderService, ICustomerService customerService)
		{
			_navigationService = navigationService;
            _customerService = customerService;
            _orderService = orderService;
            SetOrder(id).Await();
		}

        private void GoBack()
        {
            _navigationService.GoBack();
        }

        private async Task SetOrder(string id)
        {
            Order = await _orderService.GetByNumber(id);
            _orderBackup = new OrderModel(Order);
        }

        private void EditCustomer()
        {
            var vm = new PickCustomerViewModel(_customerService, _navigationService, customer =>
            {
                Order.Customer = customer;
                NotifyPropertyChanged(nameof(Order));
                NotifyPropertyChanged(nameof(CustomerButtonText));
            });

            _navigationService.ShowDialog(vm, callback: result => { });
        }
    }
}
