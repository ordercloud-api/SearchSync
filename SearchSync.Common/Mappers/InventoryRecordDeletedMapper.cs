using SearchSync.Common.Exceptions;
using SearchSync.Common.Models;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Mappers
{
    public interface IInventoryRecordDeletedMapper : ISearchMapper<InventoryRecordDeletedMessage>
    {
    }

    public class InventoryRecordDeletedMapper : SearchMapper<InventoryRecordDeletedMessage>, IInventoryRecordDeletedMapper
    {
        public override string GetDocumentId(InventoryRecordDeletedMessage message)
        {
            return message.ID;
        }

        public override void Validate(InventoryRecordDeletedMessage message)
        {
            var errors = new List<MappingValidationError>();

            if (message == null)
            {
                errors.Add(new MappingValidationError("[InventoryRecordDelete] Message is empty"));
                throw new MappingValidationException(errors);
            }

            if (string.IsNullOrEmpty(message.ID))
            {
                errors.Add(new MappingValidationError("[InventoryRecordDelete] ID is required"));
            }

            if (errors.Any())
            {
                throw new MappingValidationException(errors);
            }
        }

        public override SearchDocument MapToSearchDocument(InventoryRecordDeletedMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
