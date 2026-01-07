using SearchSync.Common.Exceptions;
using SearchSync.Common.Models;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Mappers
{
    public interface ISupplierUpdatedMapper : ISearchMapper<SupplierUpdatedMessage>
    {
    }

    public class SupplierUpdatedMapper : SearchMapper<SupplierUpdatedMessage>, ISupplierUpdatedMapper
    {
        public override string GetDocumentId(SupplierUpdatedMessage message)
        {
            return message.ID;
        }

        public override void Validate(SupplierUpdatedMessage message)
        {
            var errors = new List<MappingValidationError>();

            if (message == null)
            {
                errors.Add(new MappingValidationError("[SupplierUpdate] Message is empty"));
                throw new MappingValidationException(errors);
            }

            if (string.IsNullOrEmpty(message.ID))
            {
                errors.Add(new MappingValidationError("[SupplierUpdate] ID is required"));
            }

            if (errors.Any())
            {
                throw new MappingValidationException(errors);
            }
        }

        public override SearchDocument MapToSearchDocument(SupplierUpdatedMessage message)
        {
            if (message == null)
            {
                throw new MappingException("[SupplierUpdate] Message is empty");
            }

            var document = new SearchDocument
            {
                DocumentId = message.ID
            };

            try
            {
                document.AddField("name", message.Name);
                document.AddField("active", message.Active);

                if (message.xp != null)
                {
                    document.AddField("xp_examplefield", message.xp.ExampleField);
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
