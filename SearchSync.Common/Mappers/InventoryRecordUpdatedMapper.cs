using SearchSync.Common.Exceptions;
using SearchSync.Common.Models;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Mappers
{
    public interface IInventoryRecordUpdatedMapper : ISearchMapper<InventoryRecordUpdatedMessage>
    {
    }

    public class InventoryRecordUpdatedMapper : SearchMapper<InventoryRecordUpdatedMessage>, IInventoryRecordUpdatedMapper
    {
        public override string GetDocumentId(InventoryRecordUpdatedMessage message)
        {
            // The document ID should correspond to the product this inventory belongs to.
            return message.ProductID;
        }

        public override void Validate(InventoryRecordUpdatedMessage message)
        {
            var errors = new List<MappingValidationError>();

            if (message == null)
            {
                errors.Add(new MappingValidationError("[InventoryRecordUpdate] Message is empty"));
                throw new MappingValidationException(errors);
            }

            if (string.IsNullOrEmpty(message.ProductID))
            {
                errors.Add(new MappingValidationError("[InventoryRecordUpdate] ProductID is required"));
            }

            if (errors.Any())
            {
                throw new MappingValidationException(errors);
            }
        }

        public override SearchDocument MapToSearchDocument(InventoryRecordUpdatedMessage message)
        {
            if (message == null)
            {
                throw new MappingException("[InventoryRecordUpdate] Message is empty");
            }

            var document = new SearchDocument
            {
                DocumentId = message.ProductID
            };

            try
            {
                if (message.InventoryRecords != null && message.InventoryRecords.Any())
                {
                    var mappedInventoryList = message.InventoryRecords.Select(inv =>
                    {
                        var inventoryDict = new Dictionary<string, object>
                        {
                            { "id", inv.ID },
                            { "ownerid", inv.OwnerID },
                            { "addressid", inv.AddressID },
                            { "ordercanexceed", inv.OrderCanExceed },
                            { "quantityavailable", inv.QuantityAvailable },
                            { "zip", inv.Zip }
                        };

                        if (inv.xp != null)
                        {
                            inventoryDict.Add("xp_examplefield", inv.xp.ExampleField);
                        }
                        return inventoryDict;
                    }).ToList();
                    document.AddField("inventoryrecords", mappedInventoryList);
                }
            }
            catch (Exception ex)
            {
                throw new MappingException(ex, message);
            }

            return document;
        }
    }
}
