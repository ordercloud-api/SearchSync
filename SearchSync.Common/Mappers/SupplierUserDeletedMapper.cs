using SearchSync.Common.Exceptions;
using SearchSync.Common.Models;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Mappers
{
    public interface ISupplierUserDeletedMapper : ISearchMapper<SupplierUserDeletedMessage>
    {
    }

    public class SupplierUserDeletedMapper : SearchMapper<SupplierUserDeletedMessage>, ISupplierUserDeletedMapper
    {
        public override string GetDocumentId(SupplierUserDeletedMessage message)
        {
            return message.ID;
        }

        public override void Validate(SupplierUserDeletedMessage message)
        {
            var errors = new List<MappingValidationError>();

            if (message == null)
            {
                errors.Add(new MappingValidationError("[SupplierUserDelete] Message is empty"));
                throw new MappingValidationException(errors);
            }

            if (string.IsNullOrEmpty(message.ID))
            {
                errors.Add(new MappingValidationError("[SupplierUserDelete] ID is required"));
            }

            if (errors.Any())
            {
                throw new MappingValidationException(errors);
            }
        }

        public override SearchDocument MapToSearchDocument(SupplierUserDeletedMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
