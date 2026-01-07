using SearchSync.Common.Exceptions;
using SearchSync.Common.Models;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Mappers
{
    public interface IBuyerUpdatedMapper : ISearchMapper<BuyerUpdatedMessage>
    {
    }

    public class BuyerUpdatedMapper : SearchMapper<BuyerUpdatedMessage>, IBuyerUpdatedMapper
    {
        public override string GetDocumentId(BuyerUpdatedMessage message)
        {
            return message.ID;
        }

        public override void Validate(BuyerUpdatedMessage message)
        {
            var errors = new List<MappingValidationError>();

            if (message == null)
            {
                errors.Add(new MappingValidationError("[BuyerUpdate] Message is empty"));
                throw new MappingValidationException(errors);
            }

            if (string.IsNullOrEmpty(message.ID))
            {
                errors.Add(new MappingValidationError("[BuyerUpdate] ID is required"));
            }

            if (errors.Any())
            {
                throw new MappingValidationException(errors);
            }
        }

        public override SearchDocument MapToSearchDocument(BuyerUpdatedMessage message)
        {
            if (message == null)
            {
                throw new MappingException("[BuyerUpdate] Message is empty");
            }

            var document = new SearchDocument
            {
                DocumentId = message.ID
            };

            try
            {
                document.AddField("name", message.Name);
                document.AddField("active", message.Active);
                document.AddField("defaultcatalogid", message.DefaultCatalogID);

                // Add any custom xp fields here as needed
                if (message.xp != null)
                {
                    document.AddField("xp_somefield", message.xp.ExampleField);
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
