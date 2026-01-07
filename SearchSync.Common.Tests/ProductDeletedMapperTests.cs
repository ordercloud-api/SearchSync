using SearchSync.Common.Exceptions;
using SearchSync.Common.Mappers;
using SearchSync.Common.Models.OrderCloud.ProductSync;

namespace SearchSync.Common.Tests
{
    public class ProductDeletedMapperTests
    {
        private readonly ProductDeletedMapper _mapper;

        public ProductDeletedMapperTests()
        {
            _mapper = new ProductDeletedMapper();
        }

        [Fact]
        public void GetDocumentId_ReturnsProductId()
        {
            // Arrange
            var message = new ProductDeletedMessage { ProductId = "prod-1" };

            // Act
            var result = _mapper.GetDocumentId(message);

            // Assert
            Assert.Equal("prod-1", result);
        }

        [Fact]
        public void Validate_Throws_WhenProductIdIsNull()
        {
            // Arrange
            var message = new ProductDeletedMessage { ProductId = null };

            // Act & Assert
            var ex = Assert.Throws<MappingValidationException>(() => _mapper.Validate(message));
            Assert.Equal("ProductId is required", ex.Message);
        }

        [Fact]
        public void Validate_DoesNotThrow_WhenProductIdIsPresent()
        {
            // Arrange
            var message = new ProductDeletedMessage { ProductId = "prod-1" };

            // Act
            var exception = Record.Exception(() => _mapper.Validate(message));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void MapToSearchDocument_ThrowsNotImplementedException()
        {
            // Arrange
            var message = new ProductDeletedMessage();

            // Act & Assert
            Assert.Throws<NotImplementedException>(() => _mapper.MapToSearchDocument(message));
        }
    }
}