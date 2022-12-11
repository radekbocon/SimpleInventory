using FakeItEasy;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.ViewModels.PageViewModes;
using System.Collections.ObjectModel;
using SimpleInventory.Wpf.Services;
using SimpleInventory.Wpf.ViewModels;
using AutoMapper;
using SharpCompress.Common;

namespace SimpleInventory.Tests
{
    public class InventoryPageViewModelTests
    {
        private const string TO_BE_FIXED = "To be fixed";
        private readonly INavigationService _navigationService; 
        private readonly IInventoryService _inventoryService;
        private readonly IMapper _mapper;

        public InventoryPageViewModelTests()
        {
            _navigationService = A.Fake<INavigationService>();
            _inventoryService = A.Fake<IInventoryService>();
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
            var sut = new InventoryPageViewModel(_inventoryService, _navigationService, _mapper);

            // Act
            sut.AddNewItemCommand.Execute(null);

            // Assert
            A.CallTo(() => _navigationService.ShowModal(A<ViewModelBase>.That.IsInstanceOf(typeof(ItemDetailsViewModel)), A<Action<bool>>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();

        }

        [Fact]
        public void EditEntryCommand_WhenExecutedWithParameter_OpensInventoryReceivingPageInModalWindow()
        {
            // Arrange
            var sut = new InventoryPageViewModel(_inventoryService, _navigationService, _mapper);
            var inventoryEntry = A.Fake<InventoryEntryViewModel>();
            inventoryEntry.Id = Guid.NewGuid().ToString();

            // Act
            sut.EditEntryCommand.Execute(inventoryEntry);

            // Assert
            A.CallTo(() => _navigationService.ShowModal(A<ViewModelBase>.That.IsInstanceOf(typeof(ReceivingViewModel)), A<Action<bool>>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();

        }

        [Fact]
        public void EditEntryCommand_WhenEntryIdIsNull_ShouldNotOpenReceivingPage()
        {
            // Arrange
            var sut = new InventoryPageViewModel(_inventoryService, _navigationService, _mapper);
            var inventoryEntry = A.Fake<InventoryEntryViewModel>();
            inventoryEntry.Id = null;

            // Act
            sut.EditEntryCommand.Execute(inventoryEntry);

            // Assert
            A.CallTo(() => _navigationService.ShowModal(A<ViewModelBase>.Ignored, A<Action<bool>>.Ignored, A<double>.Ignored)).MustNotHaveHappened();

        }

        [Fact]
        public void EditEntryCommand_WhenParameterIsNull_ShouldNotExecute()
        {
            // Arrange
            var sut = new InventoryPageViewModel(_inventoryService, _navigationService, _mapper);

            // Act

            // Assert
            Assert.False(sut.EditEntryCommand.CanExecute(null));

        }
    }
}