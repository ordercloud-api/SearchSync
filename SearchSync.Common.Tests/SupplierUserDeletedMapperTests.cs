using SearchSync.Common.Exceptions;
using SearchSync.Common.Mappers;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Tests
{
    public class SupplierUserDeletedMapperTests
    {
        private readonly SupplierUserDeletedMapper _mapper;

        public SupplierUserDeletedMapperTests()
        {
            _mapper = new SupplierUserDeletedMapper();
        }

        [Fact]
        public void GetDocumentId_ReturnsId()
        {
            // Arrange
            var message = new SupplierUserDeletedMessage { ID = "test-id" };

            // Act
            var result = _mapper.GetDocumentId(message);

            // Assert
            Assert.Equal("test-id", result);
        }

        [Fact]
        public void Validate_Throws_WhenMessageIsNull()
        {
            // Act
            var ex = Assert.Throws<MappingValidationException>(() => _mapper.Validate(null));

            // Assert
            Assert.Contains("[SupplierUserDelete] Message is empty", ex.Errors.Select(e => e.Message));
        }

        [Fact]
        public void Validate_Throws_WhenIdIsMissing()
        {
            // Arrange
            var message = new SupplierUserDeletedMessage { ID = null };

            // Act
            var ex = Assert.Throws<MappingValidationException>(() => _mapper.Validate(message));

            // Assert
            Assert.Contains("[SupplierUserDelete] ID is required", ex.Errors.Select(e => e.Message));
        }

        [Fact]
        public void Validate_DoesNotThrow_WhenIdIsPresent()
        {
            // Arrange
            var message = new SupplierUserDeletedMessage { ID = "valid-id" };

            // Act
            var exception = Record.Exception(() => _mapper.Validate(message));

            // Assert
            Assert.Null(exception);
        }
    }
}