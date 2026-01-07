using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SearchSync.Common.Mappers;
using SearchSync.Common.Models;
using SearchSync.Common.Models.OrderCloud;
using SearchSync.Common.Models.OrderCloud.ProductSync;
using SearchSync.Common.Models.OrderCloud.Sync;
using SearchSync.Common.Services;
using static SearchSync.Common.Constants;

namespace SearchSync.Common.Tests
{
    public class SearchIngestionServiceTests
    {
        private readonly Mock<ISearchIngestionClient> _searchClient = new();
        private readonly Mock<IOptions<SearchIngestionServiceOptions>> _options = new();
        private readonly Mock<ILogger<SearchIngestionService>> _logger = new();
        private readonly SearchIngestionService _service;

        // Mocks for all mappers
        private readonly Mock<IProductUpdatedMapper> _productUpdatedMapper = new();
        private readonly Mock<IProductDeletedMapper> _productDeletedMapper = new();
        private readonly Mock<ICategoryUpdatedMapper> _categoryUpdatedMapper = new();
        private readonly Mock<ICategoryDeletedMapper> _categoryDeletedMapper = new();
        private readonly Mock<IAdminUserUpdatedMapper> _adminUserUpdatedMapper = new();
        private readonly Mock<IAdminUserDeletedMapper> _adminUserDeletedMapper = new();
        private readonly Mock<IBuyerUpdatedMapper> _buyerUpdatedMapper = new();
        private readonly Mock<IBuyerDeletedMapper> _buyerDeletedMapper = new();
        private readonly Mock<IBuyerUserGroupUpdatedMapper> _buyerUserGroupUpdatedMapper = new();
        private readonly Mock<IBuyerUserGroupDeletedMapper> _buyerUserGroupDeletedMapper = new();
        private readonly Mock<IBuyerUserUpdatedMapper> _buyerUserUpdatedMapper = new();
        private readonly Mock<IBuyerUserDeletedMapper> _buyerUserDeletedMapper = new();
        private readonly Mock<IInventoryRecordUpdatedMapper> _inventoryRecordUpdatedMapper = new();
        private readonly Mock<IInventoryRecordDeletedMapper> _inventoryRecordDeletedMapper = new();
        private readonly Mock<ISupplierUpdatedMapper> _supplierUpdatedMapper = new();
        private readonly Mock<ISupplierDeletedMapper> _supplierDeletedMapper = new();
        private readonly Mock<ISupplierUserUpdatedMapper> _supplierUserUpdatedMapper = new();
        private readonly Mock<ISupplierUserDeletedMapper> _supplierUserDeletedMapper = new();

        public SearchIngestionServiceTests()
        {
            _options.Setup(o => o.Value).Returns(new SearchIngestionServiceOptions
            {
                EntityProduct = "product",
                EntityCategory = "category",
                EntityAdminUser = "adminuser",
                EntityBuyer = "buyer",
                EntityBuyerUserGroup = "buyerusergroup",
                EntityBuyerUser = "buyeruser",
                EntityInventoryRecord = "inventoryrecord",
                EntitySupplier = "supplier",
                EntitySupplierUser = "supplieruser"
            });

            _service = new SearchIngestionService(
                _searchClient.Object,
                _options.Object,
                _productUpdatedMapper.Object,
                _productDeletedMapper.Object,
                _categoryUpdatedMapper.Object,
                _categoryDeletedMapper.Object,
                _adminUserUpdatedMapper.Object,
                _adminUserDeletedMapper.Object,
                _buyerUpdatedMapper.Object,
                _buyerDeletedMapper.Object,
                _buyerUserGroupUpdatedMapper.Object,
                _buyerUserGroupDeletedMapper.Object,
                _buyerUserUpdatedMapper.Object,
                _buyerUserDeletedMapper.Object,
                _inventoryRecordUpdatedMapper.Object,
                _supplierUpdatedMapper.Object,
                _supplierDeletedMapper.Object,
                _supplierUserUpdatedMapper.Object,
                _supplierUserDeletedMapper.Object,
                _logger.Object
            );
        }

        [Fact]
        public async Task ProcessEvent_ProductUpdated_CallsPutAsync()
        {
            // Arrange
            var payload = new ProductUpdatedMessage();
            _productUpdatedMapper.Setup(m => m.GetDocumentId(payload)).Returns("123");
            _productUpdatedMapper.Setup(m => m.MapToSearchDocument(payload)).Returns(new SearchDocument { });

            // Act
            await _service.ProcessEvent(Events.ProductUpdated, payload);

            // Assert
            _searchClient.Verify(s => s.PutAsync(It.IsAny<object>(), "product", "documents", "123"), Times.Once);
        }

        [Fact]
        public async Task ProcessEvent_ProductDeleted_CallsDeleteAsync()
        {
            // Arrange
            var payload = new ProductDeletedMessage();
            _productDeletedMapper.Setup(m => m.GetDocumentId(payload)).Returns("123");

            // Act
            await _service.ProcessEvent(Events.ProductDeleted, payload);

            // Assert
            _searchClient.Verify(s => s.DeleteAsync("product", "documents", "123"), Times.Once);
        }

        [Fact]
        public async Task ProcessEvent_UnknownType_Throws()
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _service.ProcessEvent("UnknownType", new object()));
        }

        [Fact]
        public async Task ProcessEvent_CategoryUpdated_CallsPutAsync()
        {
            // Arrange
            var payload = new CategoryUpdatedMessage();
            _categoryUpdatedMapper.Setup(m => m.GetDocumentId(payload)).Returns("cat-123");
            _categoryUpdatedMapper.Setup(m => m.MapToSearchDocument(payload)).Returns(new SearchDocument { });

            // Act
            await _service.ProcessEvent2(Events.CategoryUpdated, payload);

            // Assert
            _searchClient.Verify(s => s.PutAsync(It.IsAny<object>(), "category", "documents", "cat-123"), Times.Once);
        }
    }
}