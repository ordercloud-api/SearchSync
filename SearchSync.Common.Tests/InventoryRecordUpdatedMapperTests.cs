using SearchSync.Common.Exceptions;
using SearchSync.Common.Mappers;
using SearchSync.Common.Models.OrderCloud;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Tests
{
    public class InventoryRecordUpdatedMapperTests
    {
        private readonly InventoryRecordUpdatedMapper _mapper;

        public InventoryRecordUpdatedMapperTests()
        {
            _mapper = new InventoryRecordUpdatedMapper();
        }

        [Fact]
        public void GetDocumentId_ReturnsProductId()
        {
            // Arrange
            var message = new InventoryRecordUpdatedMessage { ProductID = "prod-1" };

            // Act
            var result = _mapper.GetDocumentId(message);

            // Assert
            Assert.Equal("prod-1", result);
        }

        [Fact]
        public void Validate_Throws_WhenMessageIsNull()
        {
            // Act
            var ex = Assert.Throws<MappingValidationException>(() => _mapper.Validate(null));

            // Assert
            Assert.Contains("[InventoryRecordUpdate] Message is empty", ex.Errors.Select(e => e.Message));
        }

        [Fact]
        public void Validate_Throws_WhenProductIdIsMissing()
        {
            // Arrange
            var message = new InventoryRecordUpdatedMessage { ProductID = null };

            // Act
            var ex = Assert.Throws<MappingValidationException>(() => _mapper.Validate(message));

            // Assert
            Assert.Contains("[InventoryRecordUpdate] ProductID is required", ex.Errors.Select(e => e.Message));
        }

        [Fact]
        public void Validate_DoesNotThrow_WhenProductIdIsPresent()
        {
            // Arrange
            var message = new InventoryRecordUpdatedMessage { ProductID = "prod-1" };

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
            Assert.Contains("[InventoryRecordUpdate] Message is empty", ex.Message);
        }

        [Fact]
        public void MapToSearchDocument_MapsInventoryRecords()
        {
            // Arrange
            var message = new InventoryRecordUpdatedMessage
            {
                ProductID = "prod-1",
                InventoryRecords = new List<MinimalInventoryRecord>
                {
                    new MinimalInventoryRecord
                    {
                        ID = "inv-1",
                        OwnerID = "owner-1",
                        AddressID = "addr-1",
                        OrderCanExceed = false,
                        QuantityAvailable = 10,
                        Zip = "50317",
                        xp = new InventoryXp { ExampleField = "example" }
                    }
                }
            };

            // Act
            var doc = _mapper.MapToSearchDocument(message);
            var inventoryRecords = doc.GetField<List<Dictionary<string, object>>>("inventoryrecords");

            // Assert
            Assert.Equal("prod-1", doc.DocumentId);
            Assert.NotNull(inventoryRecords);
            Assert.Single(inventoryRecords);
            Assert.Equal("inv-1", inventoryRecords[0]["id"]);
            Assert.Equal(10, inventoryRecords[0]["quantityavailable"]);
            Assert.Equal("example", inventoryRecords[0]["xp_examplefield"]);
        }
    }
}