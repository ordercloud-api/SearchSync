using Moq;
using Newtonsoft.Json;
using OrderCloud.SDK;
using SearchSync.Common.Exceptions;
using SearchSync.Common.Models.OrderCloud;
using SearchSync.Common.Models.OrderCloud.Sync;
using SearchSync.Common.Services;
using static SearchSync.Common.Constants;

namespace SearchSync.Common.Tests
{
    public class EventParserTests
    {
        private readonly EventParser _eventParser;

        public EventParserTests()
        {
            _eventParser = new EventParser();
        }

        [Fact]
        public void Parse_ValidProductUpdatedEvent_ReturnsCorrectEventTypeAndPayload()
        {
            // Arrange
            var mockContext = new Mock<IEventContext>();
            var eventType = Events.ProductUpdated;

            // Initialize a valid ProductUpdatedMessage
            var payload = new ProductUpdatedMessage
            {
                ProductID = "12345",
                UserContext = new UserContext
                {
                    ID = "user-001",
                    AnonymousId = "anon-001"
                },
                Marketplace = "US",
                DefaultPriceSchedule = new MyPriceSchedule
                {
                    PriceBreaks = new List<PriceBreak> { new PriceBreak { Price = 29.99m } },
                    Currency = "USD"
                },
                PartyPriceSchedules = new List<PartyPriceSchedule>
                {
                    new PartyPriceSchedule
                    {
                        Seller = "Seller1",
                        Party = "Party1",
                        PartyType = 1,
                        PriceSchedule = new MyPriceSchedule {
                            PriceBreaks = new List<PriceBreak>{ new PriceBreak { Price = 19.99m } },
                            Currency = "USD"
                        }
                    }
                },
                SellerDefaultPriceSchedules = new List<SellerDefaultPriceSchedule>
                {
                    new SellerDefaultPriceSchedule
                    {
                        Seller = "Seller1",
                        PriceSchedule = new MyPriceSchedule {
                            PriceBreaks = new List<PriceBreak>{ new PriceBreak { Price = 25.99m } },
                            Currency = "USD"
                        }
                    }
                },
                Catalogs = new[] { "Catalog1", "Catalog2" },
                Suppliers = new[] { "Supplier1" },
                Buyers = new[] { "Buyer1" },
                Categories = new[] { "Category1", "Category2" },
                UserGroups = new[] { "Group1" }
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);

            mockContext.Setup(c => c.GetProperty("type")).Returns(eventType);
            mockContext.Setup(c => c.Body).Returns(jsonPayload);

            // Act
            var result = _eventParser.Parse(mockContext.Object);

            // Assert
            Assert.Equal(eventType, result.EventType);
            Assert.IsType<ProductUpdatedMessage>(result.Payload);
            var parsedPayload = (ProductUpdatedMessage)result.Payload;

            // Additional assertions to check if values are set correctly
            Assert.Equal("12345", parsedPayload.ProductID);
            Assert.Equal("user-001", parsedPayload.UserContext.ID);
            Assert.Equal("US", parsedPayload.Marketplace);
            Assert.Equal(29.99m, parsedPayload.DefaultPriceSchedule.PriceBreaks[0].Price);
            Assert.Equal("USD", parsedPayload.DefaultPriceSchedule.Currency);
            Assert.Single(parsedPayload.PartyPriceSchedules);
            Assert.Equal(19.99m, parsedPayload.PartyPriceSchedules[0].PriceSchedule.PriceBreaks[0].Price);
            Assert.Equal(25.99m, parsedPayload.SellerDefaultPriceSchedules[0].PriceSchedule.PriceBreaks[0].Price);
            Assert.Equal("Seller1", parsedPayload.PartyPriceSchedules[0].Seller);
            Assert.Equal("Catalog1", parsedPayload.Catalogs[0]);
            Assert.Equal("Group1", parsedPayload.UserGroups[0]);
        }

        [Fact]
        public void Parse_MissingEventType_ThrowsEventParserException()
        {
            // Arrange
            var mockContext = new Mock<IEventContext>();
            mockContext.Setup(c => c.GetProperty("type")).Returns((string)null);

            // Act & Assert
            var exception = Assert.Throws<EventParserException>(() => _eventParser.Parse(mockContext.Object));
            Assert.Equal("Missing event type", exception.Message);
        }

        [Fact]
        public void Parse_UnknownEventType_ThrowsEventParserException()
        {
            // Arrange
            var mockContext = new Mock<IEventContext>();
            var unknownEventType = "UnknownEventType";
            mockContext.Setup(c => c.GetProperty("type")).Returns(unknownEventType);

            // Act & Assert
            var exception = Assert.Throws<EventParserException>(() => _eventParser.Parse(mockContext.Object));
            Assert.Equal($"Unknown event type: {unknownEventType}", exception.Message);
        }

        [Fact]
        public void Parse_DeserializationFailure_ThrowsEventParserException()
        {
            // Arrange
            var mockContext = new Mock<IEventContext>();
            var eventType = Events.ProductUpdated;
            var invalidJson = "{ invalidJson }";  // Malformed JSON

            mockContext.Setup(c => c.GetProperty("type")).Returns(eventType);
            mockContext.Setup(c => c.Body).Returns(invalidJson);

            // Act & Assert
            var exception = Assert.Throws<EventParserException>(() => _eventParser.Parse(mockContext.Object));
            Assert.Contains("Failed to deserialize event type", exception.Message);
        }
    }
}
