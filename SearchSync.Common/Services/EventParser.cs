using Newtonsoft.Json;
using SearchSync.Common.Exceptions;
using SearchSync.Common.Models.OrderCloud;
using SearchSync.Common.Models.OrderCloud.ProductSync;
using SearchSync.Common.Models.OrderCloud.Sync;
using static SearchSync.Common.Constants;

namespace SearchSync.Common.Services
{
    public interface IEventParser
    {
        (string EventType, object Payload) Parse(IEventContext context);
    }

    public class EventParser : IEventParser
    {
        private static readonly Dictionary<string, Type> _payloadTypeMap = new()
        {
            // Products
            [Events.ProductUpdated] = typeof(ProductUpdatedMessage),
            [Events.ProductDeleted] = typeof(ProductDeletedMessage),

            // Categories
            [Events.CategoryUpdated] = typeof(CategoryUpdatedMessage),
            [Events.CategoryDeleted] = typeof(CategoryDeletedMessage),

            // Admin Users
            [Events.AdminUserUpdated] = typeof(AdminUserUpdatedMessage),
            [Events.AdminUserDeleted] = typeof(AdminUserDeletedMessage),

            // Buyers
            [Events.BuyerUpdated] = typeof(BuyerUpdatedMessage),
            [Events.BuyerDeleted] = typeof(BuyerDeletedMessage),

            // Buyer User Groups
            [Events.BuyerUserGroupUpdated] = typeof(BuyerUserGroupUpdatedMessage),
            [Events.BuyerUserGroupDeleted] = typeof(BuyerUserGroupDeletedMessage),

            // Buyer Users
            [Events.BuyerUserUpdated] = typeof(BuyerUserUpdatedMessage),
            [Events.BuyerUserDeleted] = typeof(BuyerUserGroupDeletedMessage),

            // Inventory Records
            [Events.InventoryRecordUpdated] = typeof(InventoryRecordUpdatedMessage),
            [Events.InventoryRecordFullUpdated] = typeof(InventoryRecordDeletedMessage),

            // Suppliers
            [Events.SupplierUpdated] = typeof(SupplierUpdatedMessage),
            [Events.SupplierDeleted] = typeof(SupplierDeletedMessage),

            // Supplier Users
            [Events.SupplierUserUpdated] = typeof(SupplierUserUpdatedMessage),
            [Events.SupplierUserDeleted] = typeof(SupplierUserDeletedMessage),
        };

        public EventParser()
        {

        }

        public (string EventType, object Payload) Parse(IEventContext context)
        {
            var eventType = context.GetProperty("type");
            if (string.IsNullOrEmpty(eventType))
            {
                throw new EventParserException("Missing event type");
            }

            if (!_payloadTypeMap.TryGetValue(eventType, out var payloadType))
            {
                throw new EventParserException($"Unknown event type: {eventType}");
            }

            var payload = DeserializeEvent(context.Body, payloadType, eventType);
            return (eventType, payload);
        }

        private object DeserializeEvent(string body, Type type, string eventType)
        {
            try
            {
                var result = JsonConvert.DeserializeObject(body, type);
                if (result == null)
                {
                    throw new EventParserException($"Deserialized {eventType} event returned null");
                }
                return result;
            }
            catch (JsonException ex)
            {
                throw new EventParserException($"Failed to deserialize event type {eventType}", ex);
            }
        }
    }
}
