using FakeItEasy;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.ViewModels.PageViewModes;
using System.Collections.ObjectModel;
using SimpleInventory.Wpf.Services;
using SimpleInventory.Wpf.ViewModels;

namespace SimpleInventory.Tests
{
    public class InventoryPageViewModelTests
    {
        private const string TO_BE_FIXED = "To be fixed";
        private readonly IInventoryService _inventoryService;
        private readonly INavigationService _dialogService;

        public InventoryPageViewModelTests()
        {
            _inventoryService = A.Fake<IInventoryService>();
            _dialogService = A.Fake<INavigationService>();
        }

        [Theory(Skip = TO_BE_FIXED)]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void WhenSearchTextIsEmpty_ShouldReturnFullInventory(string searchText)
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

       
        [Theory(Skip = TO_BE_FIXED)]
        [InlineData("aaa", 1)]
        [InlineData("bb", 2)]
        [InlineData("bbc", 0)]
        public void WhenSearchTextIsNotEmpty_ShouldReturnFilteredInventory(string searchText, int expectedCount)
        {
            // Arrange
            var sut = A.Fake<InventoryPageViewModel>();
            var inventoryItem = A.Fake<InventoryEntryViewModel>();
            sut.Inventory = new ObservableCollection<InventoryEntryViewModel> {inventoryItem};

            // Act
            sut.SearchText = searchText;

            // Assert
            Assert.Equal(expectedCount, sut.Inventory.Count);
        }

        [Fact]
        public void Test()
        {
            // Arrange
            var sut = A.Fake<InventoryPageViewModel>();
            var inventoryItem = A.Fake<InventoryEntryViewModel>();
            sut.Inventory = new ObservableCollection<InventoryEntryViewModel> {inventoryItem};

            // Act
        }

        private static List<ItemModel> FakeInventory()
        {
            return new List<ItemModel>()
            {
                new ItemModel { Name = "aaa", Description = "555", ProductId = "jjj" },
                new ItemModel { Name = "bbb", Description = "bbb", ProductId = "bbb" },
                new ItemModel { Name = "BBB", Description = "BBB", ProductId = "BBB" },
                new ItemModel { Name = "ccc", Description = "ccc", ProductId = "ccc" }
            };
        }
    }
}