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
    }
}