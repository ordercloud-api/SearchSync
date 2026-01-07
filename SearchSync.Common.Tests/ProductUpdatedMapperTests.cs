using OrderCloud.SDK;
using SearchSync.Common.Exceptions;
using SearchSync.Common.Mappers;
using SearchSync.Common.Models.OrderCloud;

namespace SearchSync.Common.Tests
{
    public class ProductUpdatedMapperTests
    {
        private readonly ProductUpdatedMapper _mapper;

        public ProductUpdatedMapperTests()
        {
            _mapper = new ProductUpdatedMapper();
        }

        [Fact]
        public void GetDocumentId_ReturnsProductId()
        {
            // Arrange
            var message = new ProductUpdatedMessage { ProductID = "test-id" };

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
            Assert.Contains("message is empty", ex.Errors.Select(e => e.Message));
        }

        [Fact]
        public void Validate_Throws_WhenProductIdIsMissing()
        {
            // Arrange
            var message = new ProductUpdatedMessage { ProductID = null };

            // Act
            var ex = Assert.Throws<MappingValidationException>(() => _mapper.Validate(message));

            // Assert
            Assert.Contains("ProductId is required", ex.Errors.Select(e => e.Message));
        }

        [Fact]
        public void Validate_DoesNotThrow_WhenProductIdIsPresent()
        {
            // Arrange
            var message = new ProductUpdatedMessage { ProductID = "valid-id" };

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
            Assert.Contains("Message is empty", ex.Message);
        }

        [Fact]
        public void MapToSearchDocument_MapsBasicFields()
        {
            // Arrange
            var message = new ProductUpdatedMessage
            {
                ProductID = "p1",
                Name = "Product Name",
                Description = "Description",
                OwnerID = "owner",
                Active = true,
                xp = new ProductXp { ExampleField = "val" },
                Marketplace = "m1",
                Buyers = new string[] { "b1" },
                Suppliers = new string[] { "s1" },
                UserGroups = new string[] { "g1" },
                Catalogs = new string[] { "c1" },
                Categories = new string[] { "cat1" },
                DefaultPriceScheduleID = "ps1"
            };

            // Act
            var doc = _mapper.MapToSearchDocument(message);

            // Assert
            Assert.Equal("p1", doc.DocumentId);
            Assert.Equal("Product Name", doc.GetField<string>("name"));
            Assert.Equal("val", doc.GetField<string>("xp_examplefield"));
            Assert.Contains("b1", doc.GetField<string[]>("buyers") ?? new string[] { });
        }

        [Fact]
        public void MapToSearchDocument_MapsPriceSchedules()
        {
            // Arrange
            var schedule = new MyPriceSchedule
            {
                ID = "ps-id",
                OwnerID = "owner",
                Name = "Price Name",
                Currency = "USD",
                SaleStart = DateTime.UtcNow,
                SaleEnd = DateTime.UtcNow.AddDays(1),
                PriceBreaks = new List<PriceBreak>
                {
                    new PriceBreak { Quantity = 1, Price = 10, SalePrice = 8 }
                },
                xp = new PriceScheduleXp { ExampleField = "ps-xp" }
            };

            var message = new ProductUpdatedMessage
            {
                ProductID = "p2",
                PartyPriceSchedules = new List<PartyPriceSchedule>
                {
                    new PartyPriceSchedule
                    {
                        Seller = "seller1",
                        Party = "party1",
                        PartyType = 1,
                        PriceSchedule = schedule
                    }
                }
            };

            // Act
            var doc = _mapper.MapToSearchDocument(message);
            var partySchedules = doc.GetField<object>("partypriceschedules") as List<Dictionary<string, object>>;

            // Assert
            Assert.NotNull(partySchedules);
            Assert.Single(partySchedules);
            Assert.Equal("ps-id", partySchedules[0]["id"]);
            Assert.Equal("seller1", partySchedules[0]["seller"]);
        }
    }
}