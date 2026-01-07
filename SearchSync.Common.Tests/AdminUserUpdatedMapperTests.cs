using SearchSync.Common.Exceptions;
using SearchSync.Common.Mappers;
using SearchSync.Common.Models.OrderCloud;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Tests
{
    public class AdminUserUpdatedMapperTests
    {
        private readonly AdminUserUpdatedMapper _mapper;

        public AdminUserUpdatedMapperTests()
        {
            _mapper = new AdminUserUpdatedMapper();
        }

        [Fact]
        public void GetDocumentId_ReturnsId()
        {
            // Arrange
            var message = new AdminUserUpdatedMessage { ID = "test-id" };

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
            Assert.Contains("[AdminUserUpdate] Message is empty", ex.Errors.Select(e => e.Message));
        }

        [Fact]
        public void Validate_Throws_WhenIdIsMissing()
        {
            // Arrange
            var message = new AdminUserUpdatedMessage { ID = null };

            // Act
            var ex = Assert.Throws<MappingValidationException>(() => _mapper.Validate(message));

            // Assert
            Assert.Contains("[AdminUserUpdate] ID is required", ex.Errors.Select(e => e.Message));
        }

        [Fact]
        public void Validate_DoesNotThrow_WhenIdIsPresent()
        {
            // Arrange
            var message = new AdminUserUpdatedMessage { ID = "valid-id" };

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
            Assert.Contains("[AdminUserUpdate] Message is empty", ex.Message);
        }

        [Fact]
        public void MapToSearchDocument_MapsBasicFields()
        {
            // Arrange
            var message = new AdminUserUpdatedMessage
            {
                ID = "admin-user-1",
                Username = "admin_user",
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@test.com",
                Phone = "123-456-7890",
                Active = true,
                xp = new AdminUserXp { ExampleField = "example" }
            };

            // Act
            var doc = _mapper.MapToSearchDocument(message);

            // Assert
            Assert.Equal("admin-user-1", doc.DocumentId);
            Assert.Equal("admin_user", doc.GetField<string>("username"));
            Assert.Equal("Admin", doc.GetField<string>("firstname"));
            Assert.Equal("User", doc.GetField<string>("lastname"));
            Assert.Equal("admin@test.com", doc.GetField<string>("email"));
            Assert.Equal("123-456-7890", doc.GetField<string>("phone"));
            Assert.True(doc.GetField<bool>("active"));
            Assert.Equal("Admin", doc.GetField<string>("usertype"));
            Assert.Equal("example", doc.GetField<string>("xp_examplefield"));
        }
    }
}