using SearchSync.Common.Exceptions;
using SearchSync.Common.Mappers;
using SearchSync.Common.Models.OrderCloud;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Tests
{
    public class BuyerUserUpdatedMapperTests
    {
        private readonly BuyerUserUpdatedMapper _mapper;

        public BuyerUserUpdatedMapperTests()
        {
            _mapper = new BuyerUserUpdatedMapper();
        }

        [Fact]
        public void GetDocumentId_ReturnsId()
        {
            // Arrange
            var message = new BuyerUserUpdatedMessage { ID = "test-id" };

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
            Assert.Contains("[BuyerUserUpdate] Message is empty", ex.Errors.Select(e => e.Message));
        }

        [Fact]
        public void Validate_Throws_WhenIdIsMissing()
        {
            // Arrange
            var message = new BuyerUserUpdatedMessage { ID = null };

            // Act
            var ex = Assert.Throws<MappingValidationException>(() => _mapper.Validate(message));

            // Assert
            Assert.Contains("[BuyerUserUpdate] ID is required", ex.Errors.Select(e => e.Message));
        }

        [Fact]
        public void Validate_DoesNotThrow_WhenIdIsPresent()
        {
            // Arrange
            var message = new BuyerUserUpdatedMessage { ID = "valid-id" };

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
            Assert.Contains("[BuyerUserUpdate] Message is empty", ex.Message);
        }

        [Fact]
        public void MapToSearchDocument_MapsBasicFields()
        {
            // Arrange
            var message = new BuyerUserUpdatedMessage
            {
                ID = "buyer-user-1",
                Username = "buyer_user",
                FirstName = "Buyer",
                LastName = "User",
                Email = "buyer@test.com",
                Phone = "123-456-7890",
                Active = true,
                xp = new BuyerUserXp { ExampleField = "example" }
            };

            // Act
            var doc = _mapper.MapToSearchDocument(message);

            // Assert
            Assert.Equal("buyer-user-1", doc.DocumentId);
            Assert.Equal("buyer_user", doc.GetField<string>("username"));
            Assert.Equal("Buyer", doc.GetField<string>("firstname"));
            Assert.Equal("User", doc.GetField<string>("lastname"));
            Assert.Equal("buyer@test.com", doc.GetField<string>("email"));
            Assert.Equal("123-456-7890", doc.GetField<string>("phone"));
            Assert.True(doc.GetField<bool>("active"));
            Assert.Equal("Buyer", doc.GetField<string>("usertype"));
            Assert.Equal("example", doc.GetField<string>("xp_examplefield"));
        }
    }
}