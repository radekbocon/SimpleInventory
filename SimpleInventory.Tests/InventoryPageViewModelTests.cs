using FakeItEasy;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.ViewModels.PageViewModes;
using System.Collections.ObjectModel;
using SimpleInventory.Wpf.Services;
using SimpleInventory.Wpf.ViewModels;
using AutoMapper;
using SharpCompress.Common;
using SimpleInventory.Wpf.Factories;

namespace SimpleInventory.Tests
{
    public class InventoryPageViewModelTests
    {
        private readonly INavigationService _navigationService; 
        private readonly IInventoryService _inventoryService;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly IViewModelFactory _viewModelFactory;

        public InventoryPageViewModelTests()
        {
            _navigationService = A.Fake<INavigationService>();
            _inventoryService = A.Fake<IInventoryService>();
            _notificationService = A.Fake<INotificationService>();
            _viewModelFactory = A.Fake<IViewModelFactory>();
            _mapper = A.Fake<IMapper>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Inventory_WhenSearchTextIsEmpty_ShouldReturnFullInventory(string searchText)
        {
            // Arrange
            var sut = A.Fake<InventoryPageViewModel>();
            var inventoryItem = A.Fake<InventoryEntryViewModel>();
            sut.Inventory = new ObservableCollection<InventoryEntryViewModel> { inventoryItem };
            var expectedCount = sut.Inventory.Count;

            // Act
            sut.SearchText = searchText;

            // Assert
            Assert.Equal(expectedCount, sut.Inventory.Count);
        }

       
        [Theory]
        [InlineData("aaa", 1)]
        [InlineData("AAA", 1)]
        [InlineData("aaaa", 0)]
        [InlineData("bbb", 0)]
        public void Inventory_WhenSearchByLocation_ShouldReturnFilteredInventory(string searchText, int expectedCount)
        {
            // Arrange
            var sut = A.Fake<InventoryPageViewModel>();
            var inventoryItem = A.Fake<InventoryEntryViewModel>();
            inventoryItem.Location = "AAA";
            sut.Inventory = new ObservableCollection<InventoryEntryViewModel> { inventoryItem };

            // Act
            sut.SearchText = searchText;

            // Assert
            Assert.Equal(expectedCount, sut.Inventory.Count);
        }

        [Theory]
        [InlineData("aaa", 1)]
        [InlineData("bbb", 1)]
        [InlineData("ccc", 1)]
        [InlineData("ddd", 1)]
        [InlineData("eee", 0)]
        public void Inventory_WhenSearchByItemProperties_ShouldReturnFilteredInventory(string searchText, int expectedCount)
        {
            // Arrange
            var sut = A.Fake<InventoryPageViewModel>();
            var inventoryItem = A.Fake<InventoryEntryViewModel>();
            inventoryItem.Item = new ItemViewModel
            {
                ProductId = "aaa",
                Name = "bbb",
                Description = "ccc",
                Type = "ddd"
            };
            sut.Inventory = new ObservableCollection<InventoryEntryViewModel> { inventoryItem };

            // Act
            sut.SearchText = searchText;

            // Assert
            Assert.Equal(expectedCount, sut.Inventory.Count);
        }

        [Fact]
        public void AddNewItemCommand_WhenExecuted_OpensItemDetailsInModalWindow()
        {
            // Arrange
            var sut = new InventoryPageViewModel(_inventoryService, _navigationService, _mapper, _notificationService, _viewModelFactory);

            // Act
            sut.AddNewItemCommand.Execute(null);

            // Assert
            A.CallTo(() => _viewModelFactory.Create<ItemDetailsViewModel>()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _navigationService.ShowModal(A<ItemDetailsViewModel>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void EditEntryCommand_WhenExecutedWithParameter_OpensInventoryReceivingPageInModalWindow()
        {
            // Arrange
            var sut = new InventoryPageViewModel(_inventoryService, _navigationService, _mapper, _notificationService, _viewModelFactory);
            var inventoryEntry = A.Fake<InventoryEntryViewModel>();
            inventoryEntry.Id = Guid.NewGuid().ToString();

            // Act
            sut.ReceiveItemCommand.Execute(inventoryEntry);

            // Assert
            A.CallTo(() => _viewModelFactory.Create<ReceivingViewModel>()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _navigationService.ShowModal(A<ReceivingViewModel>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void LoadInventory_WhenExecuted_LoadsInventory()
        {
            // Arrange
            int itemsCount = 4;
            A.CallTo(() => _inventoryService.GetAllEntriesAsync()).Returns(A.CollectionOfFake<InventoryEntryModel>(itemsCount).ToList());
            A.CallTo(_mapper).WithReturnType<List<InventoryEntryViewModel>>().Returns(A.CollectionOfFake<InventoryEntryViewModel>(itemsCount).ToList());
            var sut = new InventoryPageViewModel(_inventoryService, _navigationService, _mapper, _notificationService, _viewModelFactory);

            // Act
            sut.LoadInventoryCommand.Execute(null);

            // Assert
            A.CallTo(() => _inventoryService.GetAllEntriesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(_mapper).MustHaveHappenedOnceExactly();
            Assert.Equal(itemsCount, sut.Inventory.Count);
        }

        [Fact]
        public void EditEntryCommand_WhenEntryIdIsNull_ShouldNotOpenReceivingPage()
        {
            // Arrange
            var sut = new InventoryPageViewModel(_inventoryService, _navigationService, _mapper, _notificationService, _viewModelFactory);
            var inventoryEntry = A.Fake<InventoryEntryViewModel>();
            inventoryEntry.Id = null;

            // Act
            sut.ReceiveItemCommand.Execute(inventoryEntry);

            // Assert
            A.CallTo(() => _navigationService.ShowModal(A<ViewModelBase>.Ignored, A<double>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void EditEntryCommand_WhenParameterIsNull_ShouldNotExecute()
        {
            // Arrange
            var sut = new InventoryPageViewModel(_inventoryService, _navigationService, _mapper, _notificationService, _viewModelFactory);

            // Act

            // Assert
            Assert.False(sut.ReceiveItemCommand.CanExecute(null));
        }
    }
}