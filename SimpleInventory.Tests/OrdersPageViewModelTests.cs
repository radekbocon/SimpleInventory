using AutoMapper;
using FakeItEasy;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Factories;
using SimpleInventory.Wpf.Services;
using SimpleInventory.Wpf.ViewModels;
using SimpleInventory.Wpf.ViewModels.PageViewModes;
using SimpleInventory.Wpf.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Tests
{
    public class OrdersPageViewModelTests
    {
        private readonly INavigationService _navigationService;
        private readonly IInventoryService _inventoryService;
        private readonly INotificationService _notificationService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        private readonly IViewModelFactory _viewModelFactory;

        public OrdersPageViewModelTests()
        {
            _navigationService = A.Fake<INavigationService>();
            _inventoryService = A.Fake<IInventoryService>();
            _notificationService = A.Fake<INotificationService>();
            _viewModelFactory = A.Fake<IViewModelFactory>();
            _orderService = A.Fake<IOrderService>();
            _customerService = A.Fake<ICustomerService>();
            _mapper = A.Fake<IMapper>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void OrdersPage_WhenSearchTextIsEmpty_ShouldReturnAllOrders(string searchText)
        {
            // Arrange
            var sut = A.Fake<OrdersPageViewModel>();
            var order = A.Fake<OrderSummaryViewModel>();
            sut.Orders = new ObservableCollection<OrderSummaryViewModel> { order };
            var expectedCount = sut.Orders.Count;

            // Act
            sut.SearchText = searchText;

            // Assert
            Assert.Equal(expectedCount, sut.Orders.Count);
        }

        [Theory]
        [InlineData("aaa", 1)]
        [InlineData("AAA", 1)]
        [InlineData("aaaa", 0)]
        [InlineData("bbb", 0)]
        public void OrdersPage_WhenSearchByCustomerCompanyName_ShouldReturnFilteredOrders(string searchText, int expectedCount)
        {
            // Arrange
            var sut = A.Fake<OrdersPageViewModel>();
            var order = A.Fake<OrderSummaryViewModel>();
            order.Customer = new CustomerViewModel { CompanyName = "AAA" };
            sut.Orders = new ObservableCollection<OrderSummaryViewModel> { order };

            // Act
            sut.SearchText = searchText;

            // Assert
            Assert.Equal(expectedCount, sut.Orders.Count);
        }

        [Theory]
        [InlineData("aaa", 1)]
        [InlineData("AAA", 1)]
        [InlineData("aaaa", 0)]
        [InlineData("bbb", 0)]
        public void OrdersPage_WhenSearchByCustomerName_ShouldReturnFilteredOrders(string searchText, int expectedCount)
        {
            // Arrange
            var sut = A.Fake<OrdersPageViewModel>();
            var order = A.Fake<OrderSummaryViewModel>();
            order.Customer = new CustomerViewModel { FirstName = "AAA" };
            sut.Orders = new ObservableCollection<OrderSummaryViewModel> { order };

            // Act
            sut.SearchText = searchText;

            // Assert
            Assert.Equal(expectedCount, sut.Orders.Count);
        }

        [Theory]
        [InlineData("received", 1)]
        [InlineData("Recei", 1)]
        [InlineData("aaaa", 0)]
        [InlineData("bbb", 0)]
        public void OrdersPage_WhenSearchByStatus_ShouldReturnFilteredOrders(string searchText, int expectedCount)
        {
            // Arrange
            var sut = A.Fake<OrdersPageViewModel>();
            var order = A.Fake<OrderSummaryViewModel>();
            order.Status = OrderStatus.Received;
            sut.Orders = new ObservableCollection<OrderSummaryViewModel> { order };

            // Act
            sut.SearchText = searchText;

            // Assert
            Assert.Equal(expectedCount, sut.Orders.Count);
        }

        [Fact]
        public void LoadOrdersCommand_WhenExecuted_LoadsOrders()
        {
            // Arrange
            int ordersCount = 4;
            A.CallTo(() => _orderService.GetAll()).Returns(A.CollectionOfFake<OrderSummaryModel>(ordersCount).ToList());
            A.CallTo(_mapper).WithReturnType<List<OrderSummaryViewModel>>().Returns(A.CollectionOfFake<OrderSummaryViewModel>(ordersCount).ToList());
            var sut = new OrdersPageViewModel(_customerService, _orderService, _navigationService, _inventoryService, _mapper, _notificationService, _viewModelFactory);

            // Act
            sut.LoadOrdersCommand.Execute(null);

            // Assert
            A.CallTo(() => _orderService.GetAll()).MustHaveHappenedOnceExactly();
            A.CallTo(_mapper).MustHaveHappenedOnceExactly();
            Assert.Equal(ordersCount, sut.Orders.Count);
        }

        [Fact]
        public void AddNewOrderCommand_WhenExecuted_OpensOrderDetailPage()
        {
            // Arrange
            var sut = new OrdersPageViewModel(_customerService, _orderService, _navigationService, _inventoryService, _mapper, _notificationService, _viewModelFactory);

            // Act
            sut.AddNewOrderCommand.Execute(null);

            // Assert
            A.CallTo(() => _viewModelFactory.Create<OrderDetailsViewModel>()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _navigationService.OpenPage(A<OrderDetailsViewModel>.That.IsNotNull())).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void OpenOrderCommand_WhenExecuted_OpensOrderDetailPage()
        {
            // Arrange
            var sut = new OrdersPageViewModel(_customerService, _orderService, _navigationService, _inventoryService, _mapper, _notificationService, _viewModelFactory);
            var order = new OrderSummaryViewModel { Id = Guid.NewGuid().ToString() };

            // Act
            sut.OpenOrderCommand.Execute(order);

            // Assert
            A.CallTo(() => _viewModelFactory.Create<OrderDetailsViewModel>()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _navigationService.OpenPage(A<OrderDetailsViewModel>.That.IsNotNull())).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void OpenOrderCommand_WhenOrderIsNull_ShouldDoNothing()
        {
            // Arrange
            var sut = new OrdersPageViewModel(_customerService, _orderService, _navigationService, _inventoryService, _mapper, _notificationService, _viewModelFactory);

            // Act
            sut.OpenOrderCommand.Execute(null);

            // Assert
            Assert.False(sut.OpenOrderCommand.CanExecute(null));
            A.CallTo(() => _viewModelFactory.Create<OrderDetailsViewModel>()).MustNotHaveHappened();
            A.CallTo(() => _navigationService.OpenPage(A<OrderDetailsViewModel>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void OpenOrderCommand_WhenOrderIdIsNull_ShouldDoNothing()
        {
            // Arrange
            var sut = new OrdersPageViewModel(_customerService, _orderService, _navigationService, _inventoryService, _mapper, _notificationService, _viewModelFactory);
            var order = new OrderSummaryViewModel();

            // Act
            sut.OpenOrderCommand.Execute(order);

            // Assert
            A.CallTo(() => _viewModelFactory.Create<OrderDetailsViewModel>()).MustNotHaveHappened();
            A.CallTo(() => _navigationService.OpenPage(A<OrderDetailsViewModel>.Ignored)).MustNotHaveHappened();
        }
    }
}
