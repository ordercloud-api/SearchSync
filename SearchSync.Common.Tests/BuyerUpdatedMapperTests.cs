using SearchSync.Common.Exceptions;
using SearchSync.Common.Mappers;
using SearchSync.Common.Models.OrderCloud;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Tests
{
    public class BuyerUpdatedMapperTests
    {
        private readonly BuyerUpdatedMapper _mapper;

        public BuyerUpdatedMapperTests()
        {
            _mapper = new BuyerUpdatedMapper();
        }

        [Fact]
        public void GetDocumentId_ReturnsId()
        {
            // Arrange
            var message = new BuyerUpdatedMessage { ID = "test-id" };

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
            Assert.Contains("[BuyerUpdate] Message is empty", ex.Errors.Select(e => e.Message));
        }

        [Fact]
        public void Validate_Throws_WhenIdIsMissing()
        {
            // Arrange
            var message = new BuyerUpdatedMessage { ID = null };

            // Act
            var ex = Assert.Throws<MappingValidationException>(() => _mapper.Validate(message));

            // Assert
            Assert.Contains("[BuyerUpdate] ID is required", ex.Errors.Select(e => e.Message));
        }

        [Fact]
        public void Validate_DoesNotThrow_WhenIdIsPresent()
        {
            // Arrange
            var message = new BuyerUpdatedMessage { ID = "valid-id" };

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
            Assert.Contains("[BuyerUpdate] Message is empty", ex.Message);
        }

        [Fact]
        public void MapToSearchDocument_MapsBasicFields()
        {
            // Arrange
            var message = new BuyerUpdatedMessage
            {
                ID = "b1",
                Name = "Buyer Name",
                Active = true,
                DefaultCatalogID = "cat1",
                xp = new BuyerXp { ExampleField = "val" }
            };

            // Act
            var doc = _mapper.MapToSearchDocument(message);

            // Assert
            Assert.Equal("b1", doc.DocumentId);
            Assert.Equal("Buyer Name", doc.GetField<string>("name"));
            Assert.True(doc.GetField<bool>("active"));
            Assert.Equal("cat1", doc.GetField<string>("defaultcatalogid"));
            Assert.Equal("val", doc.GetField<string>("xp_somefield"));
        }
    }
}