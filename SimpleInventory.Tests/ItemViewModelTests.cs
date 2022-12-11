using FakeItEasy;
using SimpleInventory.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace SimpleInventory.Tests
{
    public class ItemViewModelTests
    {
        private readonly ItemViewModel _sut;

        public ItemViewModelTests()
        {
            _sut = new ItemViewModel();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        public void ProductId_WhenInvalid_ValidatesWithError(string value)
        {
            // Arange

            // Act
            _sut.ProductId = value;

            // Assert
            Assert.True(_sut.HasErrors);
            Assert.True(_sut.Errors.Any());
        }

        [Theory]
        [InlineData("a")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        public void ProductId_WhenValid_ValidatesWithoutErrors(string value)
        {
            // Arange

            // Act
            _sut.ProductId = value;

            // Assert
            Assert.False(_sut.HasErrors);
            Assert.False(_sut.Errors.Any());
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        public void Name_WhenInvalid_ValidatesWithError(string value)
        {
            // Arange

            // Act
            _sut.Name = value;

            // Assert
            Assert.True(_sut.HasErrors);
            Assert.True(_sut.Errors.Any());
        }

        [Theory]
        [InlineData("a")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        public void Name_WhenValid_ValidatesWithoutErrors(string value)
        {
            // Arange

            // Act
            _sut.Name = value;

            // Assert
            Assert.False(_sut.HasErrors);
            Assert.False(_sut.Errors.Any());
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Description_WhenInvalid_ValidatesWithError(string value)
        {
            // Arange

            // Act
            _sut.Description = value;

            // Assert
            Assert.True(_sut.HasErrors);
            Assert.True(_sut.Errors.Any());
        }

        [Theory]
        [InlineData("a")]
        public void Description_WhenValid_ValidatesWithoutErrors(string value)
        {
            // Arange

            // Act
            _sut.Description = value;

            // Assert
            Assert.False(_sut.HasErrors);
            Assert.False(_sut.Errors.Any());
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        public void Type_WhenInvalid_ValidatesWithError(string value)
        {
            // Arange

            // Act
            _sut.Type = value;

            // Assert
            Assert.True(_sut.HasErrors);
            Assert.True(_sut.Errors.Any());
        }

        [Theory]
        [InlineData("a")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        public void Type_WhenValid_ValidatesWithoutErrors(string value)
        {
            // Arange

            // Act
            _sut.Type = value;

            // Assert
            Assert.False(_sut.HasErrors);
            Assert.False(_sut.Errors.Any());
        }

        [Fact]
        public void Validate_WhenCalled_ValidatesAllProperties()
        {
            // Arange
            _sut.Name = "";
            _sut.Description = "";
            _sut.Type = "";
            _sut.Type = "";
            _sut.Price = -1;

            // Act
            _sut.Validate();

            // Assert
            Assert.True(_sut.HasErrors);
            Assert.True(_sut.Errors.Count >= 5);
        }

        [Theory]
        [InlineData("aaa", "aaa", true)]
        [InlineData("aaa", "bbb", false)]
        public void Items_WhenCompared_HashCodesCompareWithSameResults(string value1, string value2, bool expectedEquals)
        {
            // Arange
            var item1 = new ItemViewModel
            {
                Id = value1,
                ProductId = value1,
                Name = value1,
                Description = value1,
                Type = value1,
                Price = 1,
            };
            var item2 = new ItemViewModel
            {
                Id = value2,
                ProductId = value2,
                Name = value2,
                Description = value2,
                Type = value2,
                Price = 1,
            };

            // Act
            var actualEquals = item1.Equals(item2);
            var actualHashCodeEquals = item1.GetHashCode() == item2.GetHashCode();

            // Assert
            Assert.Equal(actualEquals, expectedEquals);
            Assert.Equal(actualHashCodeEquals, expectedEquals);
        }
    }
}
