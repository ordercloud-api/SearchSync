using SearchSync.Common.Exceptions;
using SearchSync.Common.Models;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Mappers
{
    public interface ISupplierDeletedMapper : ISearchMapper<SupplierDeletedMessage>
    {
    }

    public class SupplierDeletedMapper : SearchMapper<SupplierDeletedMessage>, ISupplierDeletedMapper
    {
        public override string GetDocumentId(SupplierDeletedMessage message)
        {
            return message.ID;
        }

        public override void Validate(SupplierDeletedMessage message)
        {
            var errors = new List<MappingValidationError>();

            if (message == null)
            {
                errors.Add(new MappingValidationError("[SupplierDelete] Message is empty"));
                throw new MappingValidationException(errors);
            }

            if (string.IsNullOrEmpty(message.ID))
            {
                errors.Add(new MappingValidationError("[SupplierDelete] ID is required"));
            }

            if (errors.Any())
            {
                throw new MappingValidationException(errors);
            }
        }

        public override SearchDocument MapToSearchDocument(SupplierDeletedMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
