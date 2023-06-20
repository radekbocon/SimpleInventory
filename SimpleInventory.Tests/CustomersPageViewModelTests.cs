using AutoMapper;
using FakeItEasy;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Factories;
using SimpleInventory.Wpf.Services;
using SimpleInventory.Wpf.ViewModels.PageViewModes;
using SimpleInventory.Wpf.ViewModels;
using System.Collections.ObjectModel;
using SimpleInventory.Core.Models;

namespace SimpleInventory.Tests
{
    public class CustomersPageViewModelTests
    {
        private readonly INavigationService _navigationService;
        private readonly IInventoryService _inventoryService;
        private readonly INotificationService _notificationService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        private readonly IViewModelFactory _viewModelFactory;
        private CustomersPageViewModel _sut;

        public CustomersPageViewModelTests()
        {
            _navigationService = A.Fake<INavigationService>();
            _inventoryService = A.Fake<IInventoryService>();
            _notificationService = A.Fake<INotificationService>();
            _viewModelFactory = A.Fake<IViewModelFactory>();
            _orderService = A.Fake<IOrderService>();
            _customerService = A.Fake<ICustomerService>();
            _mapper = A.Fake<IMapper>();
            _sut = new CustomersPageViewModel(_customerService, _navigationService, _mapper, _viewModelFactory);
        }

        [Theory]
        [InlineData("aaa", 1)]
        [InlineData("bbb", 1)]
        [InlineData("ccc", 1)]
        [InlineData("ddd", 1)]
        [InlineData("eee", 0)]
        public void Customers_WhenSearchByCustomerName_ShouldReturnFilteredOrders(string searchText, int expectedCount)
        {
            // Arrange
            var customer = A.Fake<CustomerViewModel>();
            customer.CompanyName = "aaa";
            customer.FirstName = "bbb";
            customer.LastName = "ccc";
            customer.Addresses = new ObservableCollection<Core.Models.AddressModel> { new AddressModel { City = "ddd" } };
            _sut.Customers = new ObservableCollection<CustomerViewModel> { customer };

            // Act
            _sut.SearchText = searchText;

            // Assert
            Assert.Equal(expectedCount, _sut.Customers.Count);
        }

        [Fact]
        public void LoadOCustomersCommand_WhenExecuted_ShouldLoadCustomers()
        {
            // Arrange
            int customersCount = 4;
            A.CallTo(() => _customerService.GetAll()).Returns(A.CollectionOfFake<CustomerModel>(customersCount).ToList());
            A.CallTo(_mapper).WithReturnType<List<CustomerViewModel>>().Returns(A.CollectionOfFake<CustomerViewModel>(customersCount).ToList());

            // Act
            _sut.LoadCustomersCommand.Execute(null);

            // Assert
            A.CallTo(() => _customerService.GetAll()).MustHaveHappenedOnceExactly();
            A.CallTo(_mapper).MustHaveHappenedOnceExactly();
            Assert.Equal(customersCount, _sut.Customers.Count);
        }

        [Fact]
        public void AddNewCustomerCommand_WhenExecuted_ShouldOpenCustomerDetails()
        {
            // Arrange

            // Act
            _sut.AddNewCustomerCommand.Execute(null);

            // Assert
            A.CallTo(() => _viewModelFactory.Create<CustomerDetailsViewModel>()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _navigationService.ShowModal(A<CustomerDetailsViewModel>.That.IsNotNull(), A<double>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void OpenCustomerCommand_WhenExecuted_ShouldOpenCustomerDetails()
        {
            // Arrange
            var customer = new CustomerViewModel { Id = Guid.NewGuid().ToString() };

            // Act
            _sut.OpenCustomerCommand.Execute(customer);

            // Assert
            A.CallTo(() => _viewModelFactory.Create<CustomerDetailsViewModel>()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _navigationService.ShowModal(A<CustomerDetailsViewModel>.That.IsNotNull(), A<double>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void OpenCustomerCommand_WhenOrderIsNull_ShouldDoNothing()
        {
            // Arrange

            // Act
            _sut.OpenCustomerCommand.Execute(null);

            // Assert
            Assert.False(_sut.OpenCustomerCommand.CanExecute(null));
            A.CallTo(() => _viewModelFactory.Create<CustomerDetailsViewModel>()).MustNotHaveHappened();
            A.CallTo(() => _navigationService.ShowModal(A<CustomerDetailsViewModel>.That.IsNotNull(), A<double>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void OpenCustomerCommand_WhenOrderIdIsNull_ShouldDoNothing()
        {
            // Arrange
            var customer = new CustomerViewModel();

            // Act
            _sut.OpenCustomerCommand.Execute(customer);

            // Assert
            A.CallTo(() => _viewModelFactory.Create<OrderDetailsViewModel>()).MustNotHaveHappened();
            A.CallTo(() => _navigationService.ShowModal(A<CustomerDetailsViewModel>.That.IsNotNull(), A<double>.Ignored)).MustNotHaveHappened();
        }
    }
}
