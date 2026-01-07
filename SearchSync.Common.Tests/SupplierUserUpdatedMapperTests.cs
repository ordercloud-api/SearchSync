using SearchSync.Common.Exceptions;
using SearchSync.Common.Mappers;
using SearchSync.Common.Models.OrderCloud;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Tests
{
    public class SupplierUserUpdatedMapperTests
    {
        private readonly SupplierUserUpdatedMapper _mapper;

        public SupplierUserUpdatedMapperTests()
        {
            _mapper = new SupplierUserUpdatedMapper();
        }

        [Fact]
        public void GetDocumentId_ReturnsId()
        {
            // Arrange
            var message = new SupplierUserUpdatedMessage { ID = "test-id" };

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
            Assert.Contains("[SupplierUserUpdate] Message is empty", ex.Errors.Select(e => e.Message));
        }

        [Fact]
        public void Validate_Throws_WhenIdIsMissing()
        {
            // Arrange
            var message = new SupplierUserUpdatedMessage { ID = null };

            // Act
            var ex = Assert.Throws<MappingValidationException>(() => _mapper.Validate(message));

            // Assert
            Assert.Contains("[SupplierUserUpdate] ID is required", ex.Errors.Select(e => e.Message));
        }

        [Fact]
        public void Validate_DoesNotThrow_WhenIdIsPresent()
        {
            // Arrange
            var message = new SupplierUserUpdatedMessage { ID = "valid-id" };

            // Act
            var exception = Record.Exception(() => _mapper.Validate(message));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void MapToSearchDocument_Throws_WhenMessageIsNull()
        {
            // Act
            var ex = Assert.Throws<MappingException>(() => _mapper.MapToSearchDocument(null));

            // Assert
            Assert.Contains("[SupplierUserUpdate] Message is empty", ex.Message);
        }

        [Fact]
        public void MapToSearchDocument_MapsBasicFields()
        {
            // Arrange
            var message = new SupplierUserUpdatedMessage
            {
                ID = "sup-user-1",
                Username = "sup_user",
                FirstName = "Supplier",
                LastName = "User",
                Email = "supplier@test.com",
                Phone = "123-456-7890",
                Active = true,
                xp = new SupplierUserXp { ExampleField = "example" }
            };

            // Act
            var doc = _mapper.MapToSearchDocument(message);

            // Assert
            Assert.Equal("sup-user-1", doc.DocumentId);
            Assert.Equal("sup_user", doc.GetField<string>("username"));
            Assert.Equal("Supplier", doc.GetField<string>("firstname"));
            Assert.Equal("User", doc.GetField<string>("lastname"));
            Assert.Equal("supplier@test.com", doc.GetField<string>("email"));
            Assert.Equal("123-456-7890", doc.GetField<string>("phone"));
            Assert.True(doc.GetField<bool>("active"));
            Assert.Equal("Supplier", doc.GetField<string>("usertype"));
            Assert.Equal("example", doc.GetField<string>("xp_examplefield"));
        }
    }
}