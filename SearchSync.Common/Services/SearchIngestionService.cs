using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SearchSync.Common.Mappers;
using SearchSync.Common.Models;
using SearchSync.Common.Models.OrderCloud;
using SearchSync.Common.Models.OrderCloud.ProductSync;
using SearchSync.Common.Models.OrderCloud.Sync;
using static SearchSync.Common.Constants;

namespace SearchSync.Common.Services
{
    public interface ISearchIngestionService
    {
        Task ProcessEvent<TEventPayload>(string eventType, TEventPayload payload);
    }

    public class SearchIngestionService : ISearchIngestionService
    {
        private readonly ISearchIngestionClient _searchClient;
        private readonly SearchIngestionServiceOptions _options;

        private readonly IProductUpdatedMapper _productUpdatedMapper;
        private readonly IProductDeletedMapper _productDeletedMapper;

        private readonly ICategoryUpdatedMapper _categoryUpdatedMapper;
        private readonly ICategoryDeletedMapper _categoryDeletedMapper;

        private readonly IAdminUserUpdatedMapper _adminUserUpdatedMapper;
        private readonly IAdminUserDeletedMapper _adminUserDeletedMapper;

        private readonly IBuyerUpdatedMapper _buyerUpdatedMapper;
        private readonly IBuyerDeletedMapper _buyerDeletedMapper;

        private readonly IBuyerUserGroupUpdatedMapper _buyerUserGroupUpdatedMapper;
        private readonly IBuyerUserGroupDeletedMapper _buyerUserGroupDeletedMapper;

        private readonly IBuyerUserUpdatedMapper _buyerUserUpdatedMapper;
        private readonly IBuyerUserDeletedMapper _buyerUserDeletedMapper;

        private readonly IInventoryRecordUpdatedMapper _inventoryRecordUpdatedMapper;

        private readonly ISupplierUpdatedMapper _supplierUpdatedMapper;
        private readonly ISupplierDeletedMapper _supplierDeletedMapper;

        private readonly ISupplierUserUpdatedMapper _supplierUserUpdatedMapper;
        private readonly ISupplierUserDeletedMapper _supplierUserDeletedMapper;

        private readonly ILogger<SearchIngestionService> _logger;

        public SearchIngestionService(
            ISearchIngestionClient searchClient,
            IOptions<SearchIngestionServiceOptions> options,
            IProductUpdatedMapper productUpdatedMapper,
            IProductDeletedMapper productDeletedMapper,
            ICategoryUpdatedMapper categoryUpdatedMapper,
            ICategoryDeletedMapper categoryDeletedMapper,
            IAdminUserUpdatedMapper adminUserUpdatedMapper,
            IAdminUserDeletedMapper adminUserDeletedMapper,
            IBuyerUpdatedMapper buyerUpdatedMapper,
            IBuyerDeletedMapper buyerDeletedMapper,
            IBuyerUserGroupUpdatedMapper buyerUserGroupUpdatedMapper,
            IBuyerUserGroupDeletedMapper buyerUserGroupDeletedMapper,
            IBuyerUserUpdatedMapper buyerUserUpdatedMapper,
            IBuyerUserDeletedMapper buyerUserDeletedMapper,
            IInventoryRecordUpdatedMapper inventoryRecordUpdatedMapper,
            ISupplierUpdatedMapper supplierUpdatedMapper,
            ISupplierDeletedMapper supplierDeletedMapper,
            ISupplierUserUpdatedMapper supplierUserUpdatedMapper,
            ISupplierUserDeletedMapper supplierUserDeletedMapper,
            ILogger<SearchIngestionService> logger
        )
        {
            _options = options.Value;
            _searchClient = searchClient;

            _productUpdatedMapper = productUpdatedMapper;
            _productDeletedMapper = productDeletedMapper;

            _categoryUpdatedMapper = categoryUpdatedMapper;
            _categoryDeletedMapper = categoryDeletedMapper;

            _adminUserUpdatedMapper = adminUserUpdatedMapper;
            _adminUserDeletedMapper = adminUserDeletedMapper;

            _buyerUpdatedMapper = buyerUpdatedMapper;
            _buyerDeletedMapper = buyerDeletedMapper;

            _buyerUserGroupUpdatedMapper = buyerUserGroupUpdatedMapper;
            _buyerUserGroupDeletedMapper = buyerUserGroupDeletedMapper;

            _buyerUserUpdatedMapper = buyerUserUpdatedMapper;
            _buyerUserDeletedMapper = buyerUserDeletedMapper;

            _inventoryRecordUpdatedMapper = inventoryRecordUpdatedMapper;

            _supplierUpdatedMapper = supplierUpdatedMapper;
            _supplierDeletedMapper = supplierDeletedMapper;

            _supplierUserUpdatedMapper = supplierUserUpdatedMapper;
            _supplierUserDeletedMapper = supplierUserDeletedMapper;

            _logger = logger;
        }


        public async Task ProcessEvent<TEventPayload>(string eventType, TEventPayload payload)
        {
            _logger.LogDebug($"Begin processing event {eventType}");

            if (eventType == Events.ProductUpdated && payload is ProductUpdatedMessage productUpdatedMessage)
            {
                _productUpdatedMapper.Validate(productUpdatedMessage);
                var documentId = _productUpdatedMapper.GetDocumentId(productUpdatedMessage);
                var requestBody = _productUpdatedMapper.MapToSearchDocument(productUpdatedMessage);
                await _searchClient.PutAsync(requestBody, _options.EntityProduct, "documents", documentId);
            }
            else if (eventType == Events.ProductDeleted && payload is ProductDeletedMessage productDeletedMessage)
            {
                _productDeletedMapper.Validate(productDeletedMessage);
                var documentId = _productDeletedMapper.GetDocumentId(productDeletedMessage);
                await _searchClient.DeleteAsync(_options.EntityProduct, "documents", documentId);
            }

            else if (eventType == Events.CategoryUpdated && payload is CategoryUpdatedMessage categoryUpdatedMessage)
            {
                _categoryUpdatedMapper.Validate(categoryUpdatedMessage);
                var documentId = _categoryUpdatedMapper.GetDocumentId(categoryUpdatedMessage);
                var requestBody = _categoryUpdatedMapper.MapToSearchDocument(categoryUpdatedMessage);
                await _searchClient.PutAsync(requestBody, _options.EntityCategory, "documents", documentId);
            }
            else if (eventType == Events.CategoryDeleted && payload is CategoryDeletedMessage categoryDeletedMessage)
            {
                _categoryDeletedMapper.Validate(categoryDeletedMessage);
                var documentId = _categoryDeletedMapper.GetDocumentId(categoryDeletedMessage);
                await _searchClient.DeleteAsync(_options.EntityCategory, "documents", documentId);
            }

            else if (eventType == Events.AdminUserUpdated && payload is AdminUserUpdatedMessage adminUserUpdatedMessage)
            {
                _adminUserUpdatedMapper.Validate(adminUserUpdatedMessage);
                var documentId = _adminUserUpdatedMapper.GetDocumentId(adminUserUpdatedMessage);
                var requestBody = _adminUserUpdatedMapper.MapToSearchDocument(adminUserUpdatedMessage);
                await _searchClient.PutAsync(requestBody, _options.EntityAdminUser, "documents", documentId);
            }
            else if (eventType == Events.AdminUserDeleted && payload is AdminUserDeletedMessage adminUserDeletedMessage)
            {
                _adminUserDeletedMapper.Validate(adminUserDeletedMessage);
                var documentid = _adminUserDeletedMapper.GetDocumentId(adminUserDeletedMessage);
                await _searchClient.DeleteAsync(_options.EntityAdminUser, "documents", documentid);
            }

            else if (eventType == Events.BuyerUpdated && payload is BuyerUpdatedMessage buyerUpdatedMessage)
            {
                _buyerUpdatedMapper.Validate(buyerUpdatedMessage);
                var documentId = _buyerUpdatedMapper.GetDocumentId(buyerUpdatedMessage);
                var requestBody = _buyerUpdatedMapper.MapToSearchDocument(buyerUpdatedMessage);
                await _searchClient.PutAsync(requestBody, _options.EntityBuyer, "documents", documentId);
            }
            else if (eventType == Events.BuyerDeleted && payload is BuyerDeletedMessage buyerDeletedMessage)
            {
                _buyerDeletedMapper.Validate(buyerDeletedMessage);
                var documentId = _buyerDeletedMapper.GetDocumentId(buyerDeletedMessage);
                await _searchClient.DeleteAsync(_options.EntityBuyer, "documents", documentId);
            }

            else if (eventType == Events.BuyerUserGroupUpdated && payload is BuyerUserGroupUpdatedMessage buyerUserGroupUpdatedMessage)
            {
                _buyerUserGroupUpdatedMapper.Validate(buyerUserGroupUpdatedMessage);
                var documentId = _buyerUserGroupUpdatedMapper.GetDocumentId(buyerUserGroupUpdatedMessage);
                var requestBody = _buyerUserGroupUpdatedMapper.MapToSearchDocument(buyerUserGroupUpdatedMessage);
                await _searchClient.PutAsync(requestBody, _options.EntityBuyerUserGroup, "documents", documentId);
            }
            else if (eventType == Events.BuyerUserGroupDeleted && payload is BuyerUserGroupDeletedMessage buyerUserGroupDeletedMessage)
            {
                _buyerUserGroupDeletedMapper.Validate(buyerUserGroupDeletedMessage);
                var documentId = _buyerUserGroupDeletedMapper.GetDocumentId(buyerUserGroupDeletedMessage);
                await _searchClient.DeleteAsync(_options.EntityBuyerUserGroup, "documents", documentId);
            }

            else if (eventType == Events.BuyerUserUpdated && payload is BuyerUserUpdatedMessage buyerUserUpdatedMessage)
            {
                _buyerUserUpdatedMapper.Validate(buyerUserUpdatedMessage);
                var documentId = _buyerUserUpdatedMapper.GetDocumentId(buyerUserUpdatedMessage);
                var requestBody = _buyerUserUpdatedMapper.MapToSearchDocument(buyerUserUpdatedMessage);
                await _searchClient.PutAsync(requestBody, _options.EntityBuyerUser, "documents", documentId);
            }
            else if (eventType == Events.BuyerUserDeleted && payload is UserDeletedMessage buyerUserDeletedMessage)
            {
                _buyerUserDeletedMapper.Validate(buyerUserDeletedMessage);
                var documentId = _buyerUserDeletedMapper.GetDocumentId(buyerUserDeletedMessage);
                await _searchClient.DeleteAsync(_options.EntityBuyerUser, "documents", documentId);
            }

            else if ((eventType == Events.InventoryRecordUpdated || eventType == Events.InventoryRecordFullUpdated) && payload is InventoryRecordUpdatedMessage inventoryUpdatedMessage)
            {
                _inventoryRecordUpdatedMapper.Validate(inventoryUpdatedMessage);
                var documentId = _inventoryRecordUpdatedMapper.GetDocumentId(inventoryUpdatedMessage);
                var requestBody = _inventoryRecordUpdatedMapper.MapToSearchDocument(inventoryUpdatedMessage);
                await _searchClient.PutAsync(requestBody, _options.EntityInventoryRecord, "documents", documentId);
            }
            else if (eventType == Events.SupplierUpdated && payload is SupplierUpdatedMessage supplierUpdatedMessage)
            {
                _supplierUpdatedMapper.Validate(supplierUpdatedMessage);
                var documentId = _supplierUpdatedMapper.GetDocumentId(supplierUpdatedMessage);
                var requestBody = _supplierUpdatedMapper.MapToSearchDocument(supplierUpdatedMessage);
                await _searchClient.PutAsync(requestBody, _options.EntitySupplier, "documents", documentId);
            }
            else if (eventType == Events.SupplierDeleted && payload is SupplierDeletedMessage supplierDeletedMessage)
            {
                _supplierDeletedMapper.Validate(supplierDeletedMessage);
                var documentId = _supplierDeletedMapper.GetDocumentId(supplierDeletedMessage);
                await _searchClient.DeleteAsync(_options.EntitySupplier, "documents", documentId);
            }
            else if (eventType == Events.SupplierUserUpdated && payload is SupplierUserUpdatedMessage supplierUserUpdatedMessage)
            {
                _supplierUserUpdatedMapper.Validate(supplierUserUpdatedMessage);
                var documentId = _supplierUserUpdatedMapper.GetDocumentId(supplierUserUpdatedMessage);
                var requestBody = _supplierUserUpdatedMapper.MapToSearchDocument(supplierUserUpdatedMessage);
                await _searchClient.PutAsync(requestBody, _options.EntitySupplierUser, "documents", documentId);
            }
            else if (eventType == Events.SupplierUserDeleted && payload is SupplierUserDeletedMessage supplierUserDeletedMessage)
            {
                _supplierUserDeletedMapper.Validate(supplierUserDeletedMessage);
                var documentId = _supplierUserDeletedMapper.GetDocumentId(supplierUserDeletedMessage);
                await _searchClient.DeleteAsync(_options.EntitySupplierUser, "documents", documentId);
            }

            else
            {
                throw new Exception($"Unexpected payload type for event {eventType}. Payload type: {payload?.GetType()?.Name}. Payload value: {JsonConvert.SerializeObject(payload)}");
            }
        }
    }
}
