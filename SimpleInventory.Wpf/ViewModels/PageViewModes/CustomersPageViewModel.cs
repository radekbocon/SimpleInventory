using AutoMapper;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleInventory.Wpf.ViewModels.PageViewModes
{
    public class CustomersPageViewModel : PageViewModel
    {
        public string Name { get; set; } = "Customers";

        private readonly ICustomerService _customerService;
        private readonly INavigationService _dialogService;
        private readonly IMapper _mapper;

        private ObservableCollection<CustomerViewModel> _customers;
        private ObservableCollection<CustomerViewModel> _filteredCustomers;
        private ICommand _loadCustomersCommand;
        private ICommand _deleteCustomerCommand;
        private ICommand _addNewCustomerCommand;
        private ICommand _openCustomerCommand;
        private string _searchTerxt;
        private bool _isBusy;

        public CustomersPageViewModel(ICustomerService customerService, INavigationService dialogService, IMapper mapper)
        {
            _customerService = customerService;
            _dialogService = dialogService;
            _mapper = mapper;
            //GenerateFakeCustomers();
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
                NotifyPropertyChanged(nameof(Customers));
            }
        }

        public ObservableCollection<CustomerViewModel> Customers
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SearchText) || SearchText == string.Empty)
                {
                    return _customers;
                }

                IEnumerable<CustomerViewModel> list = FilterCustomers();
                _filteredCustomers = new ObservableCollection<CustomerViewModel>(list);
                return _filteredCustomers;
            }
            set
            {
                SetProperty(ref _customers, value);
            }
        }

        public ICommand LoadCustomersCommand
        {
            get
            {
                if (_loadCustomersCommand == null)
                {
                    _loadCustomersCommand = new RelayCommand(
                        async p => await GetCustomers(),
                        p => true);
                }

                return _loadCustomersCommand;
            }
        }

        public ICommand DeleteCustomerCommand
        {
            get
            {
                if (_deleteCustomerCommand == null)
                {
                    _deleteCustomerCommand = new RelayCommand(
                        async p => await DeleteCustomer((CustomerViewModel)p),
                        p => p is CustomerViewModel);
                }

                return _deleteCustomerCommand;
            }
        }

        public ICommand AddNewCustomerCommand
        {
            get
            {
                if (_addNewCustomerCommand == null)
                {
                    _addNewCustomerCommand = new RelayCommand(
                        async p => await AddNewCustomer(),
                        p => true);
                }

                return _addNewCustomerCommand;
            }
        }

        public ICommand OpenCustomerCommand
        {
            get
            {
                if (_openCustomerCommand == null)
                {
                    _openCustomerCommand = new RelayCommand(
                        async p => await EditCustomer((CustomerViewModel)p),
                        p => p is CustomerViewModel);
                }

                return _openCustomerCommand;
            }
        }

        private IEnumerable<CustomerViewModel> FilterCustomers()
        {
            return from i in _customers
                   where i.CompanyName != null && i.CompanyName.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
                         i.FirstName != null && i.FirstName.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
                         i.LastName != null && i.LastName.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
                         i.Addresses != null && i.Addresses.Any(x => 
                            x.Line1 != null && x.Line1.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
                            x.Line2 != null && x.Line2.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
                            x.Country != null && x.Country.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
                            x.City != null && x.City.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ||
                            x.PostCode != null && x.PostCode.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase))
                   select i;
        }

        private async Task GetCustomers()
        {
            ShowBusyIndicator();
            var list = await _customerService.GetAll();
            var vmList = _mapper.Map<List<CustomerViewModel>>(list);
            Customers = new ObservableCollection<CustomerViewModel>(vmList);
            IsBusy = false;
        }

        private async Task DeleteCustomer(CustomerViewModel customer)
        {
            if (customer == null) return;

            bool delete = false;
            var vm = new YesNoDialogViewModel(_dialogService, "Delete this record?");
            _dialogService.ShowDialog(viewModel: vm, dialogWidth: 250, callback: result =>
            {
                delete = result;
            });

            if (delete)
            {
                var customerModel = _mapper.Map<CustomerModel>(customer);   
                await _customerService.DeleteOne(customerModel)
                    .ContinueWith(async t => await GetCustomers());
            }
        }

        private async Task AddNewCustomer()
        {
            bool save = false;
            var vm = new CustomerDetailsViewModel(_customerService, _dialogService, _mapper);
            _dialogService.ShowModal(vm, callback => 
            {
                save = callback;
            });

            if (save)
            {
                await GetCustomers(); 
            }
        }

        private async Task EditCustomer(CustomerViewModel customer)
        {
            if (customer.Id == null) return;

            bool save = false;
            var vm = new CustomerDetailsViewModel(customer.Id, _customerService, _dialogService, _mapper);
            _dialogService.ShowModal(vm, result =>
            {
                save = result;
            });

            if (save)
            {
                await GetCustomers();
            }
        }

        private void ShowBusyIndicator()
        {
            if (Customers == null)
            {
                IsBusy = true;
            }
        }

        private void GenerateFakeCustomers()
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

            var list = customer.Generate(1150);

            _customerService.UpsertMany(list);

        }
    }
}
