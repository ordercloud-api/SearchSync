using SearchSync.Common.Exceptions;
using SearchSync.Common.Mappers;
using SearchSync.Common.Models.OrderCloud;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Tests
{
    public class CategoryUpdatedMapperTests
    {
        private readonly CategoryUpdatedMapper _mapper;

        public CategoryUpdatedMapperTests()
        {
            _mapper = new CategoryUpdatedMapper();
        }

        [Fact]
        public void GetDocumentId_ReturnsId()
        {
            // Arrange
            var message = new CategoryUpdatedMessage { ID = "test-id" };

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
            Assert.Contains("[CategoryUpdate] Message is empty", ex.Errors.Select(e => e.Message));
        }

        [Fact]
        public void Validate_Throws_WhenIdIsMissing()
        {
            // Arrange
            var message = new CategoryUpdatedMessage { ID = null };

            // Act
            var ex = Assert.Throws<MappingValidationException>(() => _mapper.Validate(message));

            // Assert
            Assert.Contains("[CategoryUpdate] ID is required", ex.Errors.Select(e => e.Message));
        }

        [Fact]
        public void Validate_DoesNotThrow_WhenIdIsPresent()
        {
            // Arrange
            var message = new CategoryUpdatedMessage { ID = "valid-id" };

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
            Assert.Contains("[CategoryUpdate] Message is empty", ex.Message);
        }

        [Fact]
        public void MapToSearchDocument_MapsBasicFields()
        {
            // Arrange
            var message = new CategoryUpdatedMessage
            {
                ID = "cat-1",
                Name = "Category 1",
                Description = "Category Description",
                Active = true,
                ParentID = "parent-cat",
                xp = new CategoryXp { ExampleField = "example" }
            };

            // Act
            var doc = _mapper.MapToSearchDocument(message);

            // Assert
            Assert.Equal("cat-1", doc.DocumentId);
            Assert.Equal("Category 1", doc.GetField<string>("name"));
            Assert.Equal("Category Description", doc.GetField<string>("description"));
            Assert.True(doc.GetField<bool>("active"));
            Assert.Equal("parent-cat", doc.GetField<string>("parentid"));
            Assert.Equal("example", doc.GetField<string>("xp_examplefield"));
        }
    }
}